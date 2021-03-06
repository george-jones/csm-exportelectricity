﻿using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

namespace ExportElectricityMod
{
	public static class ExpmHolder
	{
		// because c# doesn't let you have bare variables in a namespace
		private static Exportable.ExportableManager expm = null;
        public static UIComponent IncomePanel;
        public static float buttonX;
        public static float buttonY;
        public static UIView view;

        public static Exportable.ExportableManager get()
		{
			if (expm == null)
			{
				expm = new Exportable.ExportableManager ();
			}
			return expm;
		}
	}

	public static class Debugger
	{
		// Debugger.Write appends to a text file.  This is here because Debug.Log wasn't having any effect
		// when called from OnUpdateMoneyAmount.  Maybe a Unity thing that event handlers can't log?  I dunno.
		public static bool enabled = false; // don't commit
		public static void Write(String s)
		{
			if (!enabled)
			{
				return;
			}

			using (System.IO.FileStream file = new System.IO.FileStream("ExportElectricityModDebug.txt", FileMode.Append)) {
				StreamWriter sw = new StreamWriter(file);
				sw.WriteLine(s);
    	       	sw.Flush();
    	    }
		}
	}

	public class ExportElectricity : IUserMod
	{
		public string Name 
		{
			get { return "Export Electricity Mod"; }
		}

		public string Description 
		{
			get { return "Earn money for unused electricity and (optionally) other production."; }
		}

		public void OnSettingsUI(UIHelperBase helper)
		{
			UIHelperBase group = helper.AddGroup("Check to enable income from excess capacity");
			ExpmHolder.get().AddOptions (group);			
		}
	}

	public class EconomyExtension : EconomyExtensionBase
	{
		private bool updated = false;
		private System.DateTime prevDate;

		public override long OnUpdateMoneyAmount(long internalMoneyAmount)
		{			
            try
            {
                DistrictManager DMinstance = Singleton<DistrictManager>.instance;
                Array8<District> dm_array = DMinstance.m_districts;
                District d;
	            
	            Debugger.Write("\r\n== OnUpdateMoneyAmount ==");

				double sec_per_day = 75600.0; // for some reason
				double sec_per_week = 7 * sec_per_day;
				double week_proportion = 0.0;
				int export_earnings = 0;
				int earnings_shown = 0;

 				if (dm_array == null)
                {
                	Debugger.Write("early return, dm_array is null");
                    return internalMoneyAmount;
                }

                d = dm_array.m_buffer[0];

				if (!updated) {
					updated = true;
					prevDate = this.managers.threading.simulationTime;
					Debugger.Write("first run");
				} else {
					System.DateTime newDate = this.managers.threading.simulationTime;
					System.TimeSpan timeDiff = newDate.Subtract (prevDate);
					week_proportion = (((double) timeDiff.TotalSeconds) / sec_per_week);
					if (week_proportion > 0.0) {
						Debugger.Write("proportion: " + week_proportion.ToString());
						EconomyManager EM = Singleton<EconomyManager>.instance;
						if (EM != null) {
							// add income							
							export_earnings = (int) ExpmHolder.get().CalculateIncome(d, week_proportion);
							earnings_shown = export_earnings / 100;
							Debugger.Write("Total earnings: " + earnings_shown.ToString());
						EM.AddResource(EconomyManager.Resource.PublicIncome,
								export_earnings,
								ItemClass.Service.None,
								ItemClass.SubService.None,
								ItemClass.Level.None);
						}
					} else {
						Debugger.Write("week_proportion zero");
					}
					prevDate = newDate;
				}	            	
			}
	        catch (Exception ex)
	        {
	        	// shouldn't happen, but if it does, start logging
	        	Debugger.Write("Exception " + ex.Message.ToString());
	        }
			return internalMoneyAmount;
		}
	}

	public class ExportLoading : LoadingExtensionBase
	{
        private GameObject ExportUIObj;

        public override void OnLevelLoaded(LoadMode mode)
		{
            if (ExportUIObj == null)
            {
                if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame)
                {
                    ExportUIObj = new GameObject();
                    ExportUIObj.AddComponent<ExportUI>();
                }
            }

            UIView view = UIView.GetAView();
            ExpmHolder.view = view;
            var c = view.FindUIComponent("IncomePanel");
            ExpmHolder.IncomePanel = c;

            var pos = c.absolutePosition;
            ExpmHolder.buttonX = (pos.x + c.width) * view.inputScale - 2;
            ExpmHolder.buttonY = (pos.y) * view.inputScale;
        }

		public override void OnLevelUnloading()
		{
            if (ExportUIObj != null)
            {
                GameObject.Destroy(ExportUIObj);
                ExportUIObj = null;
            }
        }
	}

    public class ExportUI : MonoBehaviour
    {
        private Rect windowRect = new Rect(Screen.width - 300, Screen.height - 450, 300, 300);
        private bool showingWindow = false;
        private bool uiSetup = false;
        private UIUtils.ImageButton button;
        private UIComponent tb;

        private void SetupUI()
        {
            uiSetup = true;
            UIComponent policies = ExpmHolder.view.FindUIComponent("Policies");
            tb = policies.parent; // TSBar/MainToolstrip
            string[] imgtypes = new string[] { "normalBg", "disabledBg", "hoveredBg", "pressedBg", "focusedBg", "normalFg", "pressedFg" };

            button = tb.AddUIComponent<UIUtils.ImageButton>();
            Debugger.Write(button.SetDetail("expinc", "exporticon.png", "Exports Income", 32, 42, imgtypes));

            button.eventClick += new MouseEventHandler(buttonClick);
            
        }

        private void buttonClick(UIComponent sender, UIMouseEventParameter e)
        {
            showingWindow = !showingWindow;
        }

        void OnGUI()
        {
            if (ExpmHolder.view.enabled)
            {
                if (!uiSetup)
                {
                    SetupUI();
                }

                if (showingWindow)
                {
                    windowRect = GUILayout.Window(314, windowRect, ShowExportIncomeWindow, "Weekly Income from Exports");
                }
            }
        }

        void ShowExportIncomeWindow(int windowID)
        {
            var em = ExpmHolder.get();
            SortedDictionary<string, Exportable.Exportable> exportables = em.GetExportables();
            var en = exportables.GetEnumerator();
            int totalEarned = 0;

            while (en.MoveNext())
            {
                var c = en.Current.Value;
                if (c.GetEnabled())
                {
                    int earned = (int)(c.LastWeeklyEarning / 100.0);
                    totalEarned += earned;

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(en.Current.Value.Description);
                    GUILayout.FlexibleSpace();
                    GUI.contentColor = Color.white;
                    GUILayout.Label(earned.ToString());
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Total");
            GUILayout.FlexibleSpace();
            GUILayout.Label(totalEarned.ToString());
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();            
            if (GUILayout.Button("Close"))
            {
                button.state = UIButton.ButtonState.Normal;
                showingWindow = false;
            }
            GUILayout.EndHorizontal();
            
            GUI.DragWindow();
        }

    }
}