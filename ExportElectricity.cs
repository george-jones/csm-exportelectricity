using ICities;
using ColossalFramework;
using ColossalFramework.Plugins;
using UnityEngine;
using System.IO;

namespace ExportElectricityMod
{
	public class ExportElectricity : IUserMod {

		public string Name 
		{
			get { return "Export Electricity Mod"; }
		}

		public string Description 
		{
			get { return "Earn money for unused electricity.  Only a modest income for power sources other than the Fusion power plant."; }
		}
	}

	public class EconomyExtension : EconomyExtensionBase
	{
		private bool updated = false;
		private System.DateTime prevDate;

		public override long OnUpdateMoneyAmount(long internalMoneyAmount)
		{			
			DistrictManager[] dm_array = UnityEngine.Object.FindObjectsOfType<DistrictManager>();
			District d;
			double capacity = 0;
			double consumption = 0;
			double pay_per_mw = 500.0;
			double sec_per_day = 75600.0; // for some reason
			double sec_per_week = 7 * sec_per_day;
			double week_proportion = 0.0;
			int export_earnings = 0;

			if (dm_array.Length <= 0) {
				return internalMoneyAmount;
			}

			d = dm_array[0].m_districts.m_buffer[0];
			capacity = ((double)d.GetElectricityCapacity ()) / 1000.0; // divide my 1000 to get megawatts
			consumption = ((double) d.GetElectricityConsumption()) / 1000.0;

			if (!updated) {
				updated = true;
				prevDate = this.managers.threading.simulationTime;
			} else {
				System.DateTime newDate = this.managers.threading.simulationTime;
				System.TimeSpan timeDiff = newDate.Subtract (prevDate);
				week_proportion = (((double) timeDiff.TotalSeconds) / sec_per_week);
				if (capacity > consumption && week_proportion > 0.0) {
					EconomyManager[] em_array = UnityEngine.Object.FindObjectsOfType<EconomyManager>();

					export_earnings = (int)(week_proportion * (capacity - consumption) * pay_per_mw);
					if (em_array.Length > 0) {
						// add income
						em_array[0].AddResource(EconomyManager.Resource.PublicIncome,
							export_earnings,
							ItemClass.Service.None,
							ItemClass.SubService.None,
							ItemClass.Level.None);							
					}
				} else {
					export_earnings = 0;
				}
				prevDate = newDate;
			}

			return internalMoneyAmount;
		}
	}

}