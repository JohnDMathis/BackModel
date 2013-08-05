namespace BackModel.source
{
    // Namespace:mystuff.coolstuff
    // ModelBase:NiceTools.Model
    public class ProjectListItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class BoardProjectMapping
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string TargetProjectId { get; set; }
        public bool UpdateCards { get; set; }
        public bool UpdateTargetItems { get; set; }
    }
	
	// End
	
	public class SomeAutoMappingConfig
	{
		public void Init()
		{
			//Mapper.CreateMap....
		}
	}
		
		
}
