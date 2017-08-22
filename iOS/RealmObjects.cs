using System.Collections.Generic;
using Contacts;
using Realms;
using Foundation;

public class Contact : RealmObject
{
	public string Identifier { get; set; }
	public string GivenName { get; set; }
	public string FamilyName { get; set; }
	public string OrganizationName { get; set; }
	public string Origin { get; set; }
	public string Note { get; set; }
	public IList<PhoneNumber> PhoneNumbers { get; }
	public IList<EmailAddress> EmailAddresses { get; }
	public IList<PostalAddress> PostalAddresses { get; }
	public IList<ContactGroup> Groups { get; }
	public Media ProfilePicture { get; set; }
	public IList<Folder> Folder { get; }
	public IList<Media> Media { get; }

	public static CNContact CNContactRepresentation(Contact contact)
	{
		CNMutableContact mutableContact = new CNMutableContact();
		mutableContact.GivenName = contact.GivenName != null ? contact.GivenName : "";
		mutableContact.FamilyName = contact.FamilyName != null ? contact.FamilyName : "";
		mutableContact.OrganizationName = contact.OrganizationName != null ? contact.OrganizationName : "";

		CNLabeledValue<CNPhoneNumber>[] phoneArray = new CNLabeledValue<CNPhoneNumber>[contact.PhoneNumbers.Count];
		foreach (var phoneNumber in contact.PhoneNumbers)
		{
			phoneArray[contact.PhoneNumbers.IndexOf(phoneNumber)] = new CNLabeledValue<CNPhoneNumber>(phoneNumber.Label, new CNPhoneNumber(phoneNumber.Number));
		}

		mutableContact.PhoneNumbers = phoneArray;

		CNLabeledValue<NSString>[] emailArray = new CNLabeledValue<NSString>[contact.EmailAddresses.Count];
		foreach (var emailAddress in contact.EmailAddresses)
		{
			emailArray[contact.EmailAddresses.IndexOf(emailAddress)] = new CNLabeledValue<NSString>(emailAddress.Label, (NSString)emailAddress.Email);
		}

		mutableContact.EmailAddresses = emailArray;

		CNLabeledValue<CNPostalAddress>[] addressArray = new CNLabeledValue<CNPostalAddress>[contact.PostalAddresses.Count];
		foreach (var postalAddress in contact.PostalAddresses)
		{
			var address = new CNMutablePostalAddress();
			address.Street = postalAddress.Street;
			address.PostalCode = postalAddress.PostalCode;
			address.City = postalAddress.City;
			address.Country = postalAddress.Country;
			address.IsoCountryCode = "DE";
			addressArray[contact.PostalAddresses.IndexOf(postalAddress)] = new CNLabeledValue<CNPostalAddress>(postalAddress.Label, address);
		}

		mutableContact.PostalAddresses = addressArray;

		if (!string.IsNullOrEmpty(contact.Note))
		{
			mutableContact.Note = contact.Note;
		}

		return mutableContact;
	}

	public override string ToString()
	{
		string phoneNumberString = "";
		foreach (var number in PhoneNumbers)
			phoneNumberString += number.Number + "\n";


		string emailString = "";
		foreach (var email in EmailAddresses)
			emailString += email.Email + "\n";

		string addressString = "";
		foreach (var address in PostalAddresses)
			addressString += address.ToString() + "\n";

		return GivenName + "\n" + FamilyName + "\n" + OrganizationName + "\n" + phoneNumberString + emailString + addressString;
	}

	public int GetMediaCount()
	{
		int count = 0;
		foreach (var folder in Folder)
		{
			count += folder.GetCount();
		}
		count += Media.Count;
		return count;
	}
}

public class PhoneNumber : RealmObject
{
	public string Number { get; set; }
	public string Label { get; set; }
}

public class EmailAddress : RealmObject
{
	public string Email { get; set; }
	public string Label { get; set; }
}

public class PostalAddress : RealmObject
{
	public string Street { get; set; }
	public string PostalCode { get; set; }
	public string City { get; set; }
	public string Country { get; set; }
	public string IsoCountryCode { get; set; }
	public string Label { get; set; }

	public override string ToString()
	{
		string output = "";

		output = this.Street + "\n";
		if (this.PostalCode.Length > 0)
			output = output + this.PostalCode + " ";
		if (this.City.Length > 0)
			output = output + this.City + "\n";
		output = output + this.Country;

		return output;
	}
}

public class ContactGroup : RealmObject
{
	public string Uuid { get; set; }
	public string Name { get; set; }
}


public class API : RealmObject
{
	public string URL { get; set; }
	public string Principal { get; set; }
	public string Type { get; set; }
	public string Timestamp { get; set; }
}

public class Media : RealmObject
{
	public string Title { get; set; }
	public string FileCreateDate { get; set; }
	public string FileAddDate { get; set; }
	public string Filename { get; set; }
	public string MimeType { get; set; }
	public string Uuid { get; set; }
}

public class Folder : RealmObject
{
	public string Title { get; set; }
	public string Uuid { get; set; }
	public IList<Folder> Subfolders { get; }
	public IList<Media> Contents { get; }

	public int GetCount()
	{
		int count = 0;
		foreach (var folder in Subfolders)
		{
			count += folder.GetCount();
		}
		count += Contents.Count;
		return count;
	}
}