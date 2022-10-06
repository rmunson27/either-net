using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads.Auxiliary;

/// <summary>
/// Represents an email address at which a person can be reached.
/// </summary>
/// <param name="Address"></param>
/// <remarks>
/// This record is used internally for testing.
/// </remarks>
internal sealed record class Email(string Address, bool IsPersonal) : ContactInformation(IsPersonal);

/// <summary>
/// Represents a phone number at which a person can be reached.
/// </summary>
/// <param name="Number"></param>
/// <remarks>
/// This record is used internally for testing.
/// </remarks>
internal sealed record class Phone(long Number, bool IsPersonal) : ContactInformation(IsPersonal);

/// <summary>
/// Represents contact information for a person.
/// </summary>
/// <param name="IsPersonal">
/// Whether or not the contact information is personal.
/// </param>
/// <remarks>
/// This record is used internally for testing.
/// </remarks>
internal abstract record class ContactInformation(bool IsPersonal) { }
