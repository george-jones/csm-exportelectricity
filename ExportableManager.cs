using ICities;
using System;
using System.Collections.Generic;

namespace Exportable
{
	public class ExportableManager
	{
		private SortedDictionary<string, Exportable> exportables;

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
		}

		public void AddExportable (Exportable exp)
		{
			exportables.Add (exp.Id, exp);
		}

		public void StoreSettings ()
		{
			// TODO
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
			foreach (var id in exportables.Keys) {
				Exportable exp = exportables [id];
				group.AddCheckbox(exp.Description, false, exp.SetEnabled);
			}
		}
	}
}

