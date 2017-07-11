using ICities;
using ColossalFramework;
using ColossalFramework.Plugins;
using UnityEngine;
using System.IO;
using System;

namespace ExportElectricityMod
{
	public static class ExpmHolder
	{
		// because c# doesn't let you have bare variables in a namespace
		private static Exportable.ExportableManager expm = null;

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
		public static bool enabled = false;
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
}