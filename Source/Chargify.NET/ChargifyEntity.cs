namespace ChargifyNET
{
    /// <summary>
    /// The model object for every ID-based object in Chargify
    /// </summary>
    public class ChargifyEntity : IChargifyEntity
    {
        /// <summary>
        /// The json/xml key for the ID field
        /// </summary>
        protected const string IDKey = "id";

        /// <summary>
        /// The unique id within Chargify
        /// </summary>
        public int ID
        {
            get
            {
                return this.m_id;
            }
        }

        /// <summary>
        /// The id value
        /// </summary>
        protected int m_id = int.MinValue;
    }

    /// <summary>
    /// The model object for every ID-based object in Chargify
    /// </summary>
    public interface IChargifyEntity
    {
        /// <summary>
        /// The unique id within Chargify
        /// </summary>
        int ID { get; }
    }
}