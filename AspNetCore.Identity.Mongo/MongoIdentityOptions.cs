namespace AspNetCore.Identity.Mongo
{
	public class MongoIdentityOptions
	{
		public string ConnectionString { get; set; } = "mongodb://Admin:Qwer!234@ds042527.mlab.com:42527/certification_system_tests";
        
	    public string UsersCollection { get; set; } = "Users";
		
	    public string RolesCollection { get; set; } = "Roles";

	    public bool UseDefaultIdentity { get; set; } = true;
	}
}