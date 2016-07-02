using ICities;
using ColossalFramework;
using ColossalFramework.Plugins;
using UnityEngine;
using System.IO;
using System;

namespace ExportElectricityMod
{
    public class ExportElectricity : IUserMod
    {

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
            try
            {
                DistrictManager DMinstance = Singleton<DistrictManager>.instance;
                Array8<District> dm_array = DMinstance.m_districts;
                District d;
                double capacity = 0;
                double consumption = 0;
                double pay_per_mw = 500.0;
                double sec_per_day = 75600.0; // for some reason
                double sec_per_week = 7 * sec_per_day;
                double week_proportion = 0.0;
                int export_earnings = 0;

                if (dm_array == null)
                {
                    return internalMoneyAmount;
                }

                d = dm_array.m_buffer[0];
                capacity = ((double)d.GetElectricityCapacity()) / 1000.0; // divide my 1000 to get megawatts
                consumption = ((double)d.GetElectricityConsumption()) / 1000.0;

                if (!updated)
                {
                    updated = true;
                    prevDate = this.managers.threading.simulationTime;
                }
                else
                {
                    System.DateTime newDate = this.managers.threading.simulationTime;
                    System.TimeSpan timeDiff = newDate.Subtract(prevDate);
                    week_proportion = (((double)timeDiff.TotalSeconds) / sec_per_week);
                    if (capacity > consumption && week_proportion > 0.0)
                    {

                        EconomyManager EM = Singleton<EconomyManager>.instance;

                        export_earnings = (int)(week_proportion * (capacity - consumption) * pay_per_mw);
                        if (EM != null)
                        {                            
                            // add income
                            EM.AddResource(EconomyManager.Resource.PublicIncome,
                                export_earnings,
                                ItemClass.Service.None,
                                ItemClass.SubService.None,
                                ItemClass.Level.None);
                        }
                    }
                    else
                    {
                        export_earnings = 0;
                    }
                    prevDate = newDate;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("ExportElectricity: Exception " + ex.Message.ToString());
            }
            return internalMoneyAmount;

        }

    }
}