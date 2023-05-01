using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange.Domain.ValueObjects;
public class Address : ValueObject
{
    [AllowNull]
    public String Street { get; private set; }
    [AllowNull]
    public String City { get; private set; }
    [AllowNull]
    public String State { get; private set; }
    [AllowNull]
    public String Country { get; private set; }
    [AllowNull]
    public String ZipCode { get; private set; }

    public Address() { }

    public Address(string street, string city, string state, string country, string zipcode)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipcode;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        // Using a yield return statement to return each element one at a time
        yield return Street;
        yield return City;
        yield return State;
        yield return Country;
        yield return ZipCode;
    }
}
