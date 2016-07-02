using UnityEngine;
using System;

namespace Exportable
{
	public static class Ids
	{
		public const string CREMATION = "C";
		public const string ELEMENTARY = "ED1";
		public const string HIGH_SCHOOL = "ED2";
		public const string UNIVERSITY = "ED3";
		public const string ELECTRICITY = "E";
		public const string HEALTH = "H";
		public const string HEAT = "HT";
		public const string INCINERATION = "I";
		public const string JAIL = "J";
		public const string SEWAGE = "S";
		public const string WATER = "W";
	}

	abstract public class Exportable
	{
		public string Id;
		public string Description;
		private bool Enabled;
		private ExportableManager Expm;
		private double Rate;

		public Exportable (ExportableManager inExpm, string inId, string inDescription, double inRate)
		{			
			Id = inId;
			Description = inDescription;
			Enabled = false;
			Rate = inRate;
			Expm = inExpm;
			Expm.AddExportable (this);
		}

		public void Log (String msg)
		{
			ExportElectricityMod.Debugger.Write(msg);
		}

		public void SetEnabled (bool e)
		{			
			SetEnabled (e, true);
		}

		public void SetEnabled (bool e, bool doSave)
		{
			Enabled = e;
			if (doSave) {
				Expm.StoreSettings ();
			}
		}

		public bool GetEnabled ()
		{
			return Enabled;
		}

		public double CalculateIncome(District d, double weekPortion)
		{
			double capacity = 0;
			double consumption = 0;

			capacity = ((double) this.GetCapacity (d));
			consumption = ((double) this.GetConsumption(d));

			if (capacity > consumption) {
				return weekPortion * Rate * (capacity - consumption);
			} else {
				return 0.0;
			}
		}

		abstract public double GetCapacity(District d);
		abstract public double GetConsumption(District d);
	}

	public class ExportableCremation : Exportable
	{
		public ExportableCremation(ExportableManager man) : base(man, Ids.CREMATION, "Cremation", 0.0) {
			// add more here if needed
		}

		public override double GetCapacity(District d)
		{
			return 0.0;
		}

		public override double GetConsumption(District d)
		{
			return 0.0;
		}
	}

	public class ExportableElementary : Exportable
	{
		public ExportableElementary(ExportableManager man) : base(man, Ids.ELEMENTARY, "Education - Elementary", 0.0) {
			// add more here if needed
		}

		public override double GetCapacity(District d)
		{
			return 0.0;
		}

		public override double GetConsumption(District d)
		{
			return 0.0;
		}
	}

	public class ExportableHighSchool : Exportable
	{
		public ExportableHighSchool(ExportableManager man) : base(man, Ids.HIGH_SCHOOL , "Education - High School", 0.0) {
			// add more here if needed
		}

		public override double GetCapacity(District d)
		{
			return 0.0;
		}

		public override double GetConsumption(District d)
		{
			return 0.0;
		}
	}

	public class ExportableUniversity : Exportable
	{
		public ExportableUniversity(ExportableManager man) : base(man, Ids.UNIVERSITY, "Education - University", 0.0) {
			// add more here if needed
		}

		public override double GetCapacity(District d)
		{
			return 0.0;
		}

		public override double GetConsumption(District d)
		{
			return 0.0;
		}
	}

	public class ExportableElectricity : Exportable
	{
		public ExportableElectricity(ExportableManager man) : base(man, Ids.ELECTRICITY, "Electricity", 0.5) {
			// add more here if needed
		}

		public override double GetCapacity(District d)
		{
			return (double) d.GetElectricityCapacity ();
		}

		public override double GetConsumption(District d)
		{
			return (double) d.GetElectricityConsumption();
		}
	}

	public class ExportableHealth : Exportable
	{
		public ExportableHealth(ExportableManager man) : base(man, Ids.HEALTH, "Health Care", 0.0) {
			// add more here if needed
		}

		public override double GetCapacity(District d)
		{
			return 0.0;
		}

		public override double GetConsumption(District d)
		{
			return 0.0;
		}
	}

	public class ExportableHeat : Exportable
	{
		public ExportableHeat(ExportableManager man) : base(man, Ids.HEAT, "Heat", 0.0) {
			// add more here if needed
		}

		public override double GetCapacity(District d)
		{
			return 0.0;
		}

		public override double GetConsumption(District d)
		{
			return 0.0;
		}
	}

	public class ExportableIncineration : Exportable
	{
		public ExportableIncineration(ExportableManager man) : base(man, Ids.INCINERATION, "Incineration (Garbage)", 0.0) {
			// add more here if needed
		}

		public override double GetCapacity(District d)
		{
			return 0.0;
		}

		public override double GetConsumption(District d)
		{
			return 0.0;
		}
	}

	public class ExportableJail : Exportable
	{
		public ExportableJail(ExportableManager man) : base(man, Ids.JAIL, "Jail Space", 0.0) {
			// add more here if needed
		}

		public override double GetCapacity(District d)
		{
			return 0.0;
		}

		public override double GetConsumption(District d)
		{
			return 0.0;
		}
	}

	public class ExportableSewage : Exportable
	{
		public ExportableSewage(ExportableManager man) : base(man, Ids.SEWAGE, "Sewage Treatment", 0.0) {
			// add more here if needed
		}

		public override double GetCapacity(District d)
		{
			return 0.0;
		}

		public override double GetConsumption(District d)
		{
			return 0.0;
		}
	}

	public class ExportableWater : Exportable
	{
		public ExportableWater(ExportableManager man) : base(man, Ids.WATER, "Water", 0.0) {
			// add more here if needed
		}

		public override double GetCapacity(District d)
		{
			return 0.0;
		}

		public override double GetConsumption(District d)
		{
			return 0.0;
		}
	}
}
