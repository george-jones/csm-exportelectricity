﻿using ICities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Exportable
{
	public class ExportableManager
	{
		private SortedDictionary<string, Exportable> exportables;
		public const String CONF = "ExportElectricityModConfig.txt";

		public ExportableManager ()
		{
			exportables = new SortedDictionary<string, Exportable> ();

			new ExportableCremation (this);
			new ExportableElementary (this);
			new ExportableHighSchool (this);
			new ExportableUniversity (this);
			new ExportableElectricity (this);
			new ExportableIncineration (this);
			new ExportableHealth (this);
			new ExportableHeat (this);
			new ExportableJail (this);
			new ExportableSewage (this);
			new ExportableWater (this);

			LoadSettings ();
		}

		public void Log (String msg)
		{
			Debug.Log ("[Export Electricity] " + msg);
		}

		public void AddExportable (Exportable exp)
		{
			exportables.Add (exp.Id, exp);
		}

		public void LoadSettings ()
		{
			Log ("Load Settings");
			try {
				using (System.IO.StreamReader file = 
					new System.IO.StreamReader(CONF, true))
				{
					String s = file.ReadLine ();
					String [] ids = s.Split(new char[1]{','});

					foreach (var id in ids) {
						if (exportables.ContainsKey(id)) {
							exportables[id].SetEnabled(true, false);
						}
					}
				}
			} catch (Exception e) {
				// no file? use defaults
				Log ("Using defaults: " + e.ToString());
				exportables[Ids.ELECTRICITY].SetEnabled(true);
			}
		}

		public void StoreSettings ()
		{
			Log ("Store Settings");
			try {
				using (System.IO.FileStream file =
					new System.IO.FileStream(CONF, FileMode.Create))
				{
					List<String>enabled_ids = new List<String>();
					StreamWriter sw = new StreamWriter(file);
					foreach (var pair in exportables) {
						if (pair.Value.GetEnabled()) {
							enabled_ids.Add(pair.Key);
						}
					}
					String cs = String.Join(",", enabled_ids.ToArray());
					Log ("Storing settings - enabled: " + cs);
					sw.WriteLine(cs);
					sw.Flush();
				}
			} catch (Exception e) {
				Log ("Error storing settings: " + e.ToString());
			}
		}

		public double CalculateIncome (DistrictManager dm, String id, double weekPortion)
		{
			double income = 0.0;

			if (exportables.ContainsKey (id)) {
				Exportable exp = exportables [id];
				if (exp.GetEnabled()) {
					income = exp.CalculateIncome (dm, weekPortion);
				}
			}

			return income;
		}

		public double CalculateIncome (DistrictManager dm, double weekPortion)
		{
			double total = 0.0;

			foreach (var id in exportables.Keys) {
				total += CalculateIncome (dm, id, weekPortion);
			}

			return total;
		}

		public void AddOptions (UIHelperBase group)
		{
			LoadSettings ();
			foreach (var id in exportables.Keys) {
				Exportable exp = exportables [id];
				group.AddCheckbox(exp.Description, exp.GetEnabled(), exp.SetEnabled);
			}
		}
	}
}

