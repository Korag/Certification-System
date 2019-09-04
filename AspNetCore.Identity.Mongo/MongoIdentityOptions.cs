namespace AspNetCore.Identity.Mongo
{
	public class MongoIdentityOptions
	{
		public string ConnectionString { get; set; } = "mongodb+srv://Admin:certification_admin1@mongodbcluster-wqayz.azure.mongodb.net/certification_system?retryWrites=true&w=majority";
        
	    public string UsersCollection { get; set; } = "Users";
		
	    public string RolesCollection { get; set; } = "Roles";

	    public bool UseDefaultIdentity { get; set; } = true;
	}
}