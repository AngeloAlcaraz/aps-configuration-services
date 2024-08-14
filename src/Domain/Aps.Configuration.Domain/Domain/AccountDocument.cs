namespace Aps.Configuration.Core.Domain;

public class AccountDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("Metadata")]
    [JsonIgnore]
    public Metadata Metadata { get; set; }

    [BsonElement("Account")]
    public List<Account> Account { get; set; }
}

public class Metadata
{
    [BsonElement("CorrelationId")]
    public string CorrelationId { get; set; }

    [BsonElement("Sender")]
    public string Sender { get; set; }

    [BsonElement("Destination")]
    public string Destination { get; set; }

    [BsonElement("TransactionDateTime")]
    public string TransactionDateTime { get; set; }
}

public class Account
{
    [BsonElement("Name")]
    [JsonIgnore]
    public string Name { get; set; }

    [BsonElement("Type")]
    [JsonIgnore]
    public string Type { get; set; }

    [BsonElement("SubType")]
    [JsonIgnore]
    public string SubType { get; set; }

    [BsonElement("RecordType")]
    [JsonIgnore]
    public string RecordType { get; set; }

    [BsonElement("Vendor")]
    [JsonIgnore]
    public string Vendor { get; set; }

    [BsonElement("ParentAccount")]
    [JsonIgnore]
    public string ParentAccount { get; set; }

    [BsonElement("Npi")]
    public string Npi { get; set; }

    [BsonElement("Ncpdp")]
    [JsonPropertyName("nabp")]
    public string Ncpdp { get; set; }

    [BsonElement("Status")]
    [JsonIgnore]
    public string Status { get; set; }

    [BsonElement("Bin")]
    public string Bin { get; set; }

    [BsonElement("PayerBin")]
    [JsonIgnore]
    public string PayerBin { get; set; }

    [BsonElement("PpeBin")]
    [JsonIgnore]
    public string PpeBin { get; set; }

    [BsonElement("BillCode")]
    public string BillCode { get; set; }

    [BsonElement("Region")]
    public string Region { get; set; }

    [BsonElement("Group")]
    public string Group { get; set; }

    [BsonElement("SapBillToNumber")]
    [JsonIgnore]
    public string SapBillToNumber { get; set; }

    [BsonElement("SapPayerNumber")]
    [JsonIgnore]
    public string SapPayerNumber { get; set; }

    [BsonElement("SapShipToNumber")]
    [JsonIgnore]
    public string SapShipToNumber { get; set; }

    [BsonElement("SapSoldToNumber")]
    [JsonIgnore]
    public string SapSoldToNumber { get; set; }

    [BsonElement("Address")]
    [JsonIgnore]
    public List<Address> Addresses { get; set; }
}

public class Address
{
    [BsonElement("Type")]
    public string Type { get; set; }

    [BsonElement("Line1")]
    public string Line1 { get; set; }

    [BsonElement("Line2")]
    public string Line2 { get; set; }

    [BsonElement("City")]
    public string City { get; set; }

    [BsonElement("State")]
    public string State { get; set; }

    [BsonElement("ZipCode")]
    public string ZipCode { get; set; }
}