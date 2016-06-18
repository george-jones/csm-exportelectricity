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

	public class Exportable
	{
		public string Id;
		public string Description;
		private bool Enabled;
		private ExportableManager Expm;

		public Exportable (ExportableManager inExpm, string inId, string inDescription)
		{
			Id = inId;
			Description = inDescription;
			Enabled = false;
			Expm = inExpm;
			Expm.AddExportable (this);
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

		public double CalculateIncome(DistrictManager dm, double weekPortion)
		{
			return 0.0;
		}
	}

	public class ExportableCremation : Exportable
	{
		public ExportableCremation(ExportableManager man) : base(man, Ids.CREMATION, "Cremation") {
			// add more here if needed
		}

		new public double CalculateIncome(DistrictManager dm, double weekPortion)
		{
			return 0.0;
		}
	}

	public class ExportableElementary : Exportable
	{
		public ExportableElementary(ExportableManager man) : base(man, Ids.ELEMENTARY, "Education - Elementary") {
			// add more here if needed
		}

		new public double CalculateIncome(DistrictManager dm, double weekPortion)
		{
			return 0.0;
		}
	}

	public class ExportableHighSchool : Exportable
	{
		public ExportableHighSchool(ExportableManager man) : base(man, Ids.HIGH_SCHOOL , "Education - High School") {
			// add more here if needed
		}

		new public double CalculateIncome(DistrictManager dm, double weekPortion)
		{
			return 0.0;
		}
	}

	public class ExportableUniversity : Exportable
	{
		public ExportableUniversity(ExportableManager man) : base(man, Ids.UNIVERSITY, "Education - University") {
			// add more here if needed
		}

		new public double CalculateIncome(DistrictManager dm, double weekPortion)
		{
			return 0.0;
		}
	}

	public class ExportableElectricity : Exportable
	{
		public ExportableElectricity(ExportableManager man) : base(man, Ids.ELECTRICITY, "Electricity") {
			// add more here if needed
		}

		new public double CalculateIncome(DistrictManager dm, double weekPortion)
		{
			return 0.0;
		}
	}

	public class ExportableHealth : Exportable
	{
		public ExportableHealth(ExportableManager man) : base(man, Ids.HEALTH, "Health Care") {
			// add more here if needed
		}

		new public double CalculateIncome(DistrictManager dm, double weekPortion)
		{
			return 0.0;
		}
	}

	public class ExportableHeat : Exportable
	{
		public ExportableHeat(ExportableManager man) : base(man, Ids.HEAT, "Heat") {
			// add more here if needed
		}

		new public double CalculateIncome(DistrictManager dm, double weekPortion)
		{
			return 0.0;
		}
	}

	public class ExportableIncineration : Exportable
	{
		public ExportableIncineration(ExportableManager man) : base(man, Ids.INCINERATION, "Incineration (Garbage)") {
			// add more here if needed
		}

		new public double CalculateIncome(DistrictManager dm, double weekPortion)
		{
			return 0.0;
		}
	}

	public class ExportableJail : Exportable
	{
		public ExportableJail(ExportableManager man) : base(man, Ids.JAIL, "Jail Space") {
			// add more here if needed
		}

		new public double CalculateIncome(DistrictManager dm, double weekPortion)
		{
			return 0.0;
		}
	}

	public class ExportableSewage : Exportable
	{
		public ExportableSewage(ExportableManager man) : base(man, Ids.SEWAGE, "Sewage Treatment") {
			// add more here if needed
		}

		new public double CalculateIncome(DistrictManager dm, double weekPortion)
		{
			return 0.0;
		}
	}

	public class ExportableWater : Exportable
	{
		public ExportableWater(ExportableManager man) : base(man, Ids.WATER, "Water") {
			// add more here if needed
		}

		new public double CalculateIncome(DistrictManager dm, double weekPortion)
		{
			return 0.0;
		}
	}
}
