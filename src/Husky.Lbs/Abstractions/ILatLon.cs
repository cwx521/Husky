namespace Husky.Lbs
{
	public interface ILatLon
	{
		float Lat { get; set; }
		float Lon { get; set; }
		LatLonType LatLonType { get; set; }
		string ToString();
	}
}
