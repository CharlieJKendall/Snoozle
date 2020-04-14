#### HasDqlDbType

Sets the SqlDbType that the proprety maps to for cases where the default mapping is insufficient. Generally used for date and time mappings.

**Default Mappings**

| .NET Type | SqlDbType Enum |
| ----- | ----- |
| `long` | `SqlDbType.BigInt` |
| `byte[]` | `SqlDbType.VarBinary` |
| `bool` | `SqlDbType.Bit` |
| `string` | `SqlDbType.NVarChar` |
| `DateTime` | `SqlDbType.DateTime` |
| `DateTimeOffset` | `SqlDbType.DateTimeOffset` |
| `decimal` | `SqlDbType.Decimal` |
| `double` | `SqlDbType.Float` |
| `float` | `SqlDbType.Float` |
| `int` | `SqlDbType.Int` |
| `short` | `SqlDbType.SmallInt` |
| `TimeSpan` | `SqlDbType.Time` |
| `Guid` | `SqlDbType.UniqueIdentifier` |
| `byte` | `SqlDbType.TinyInt` |
| `char` | `SqlDbType.Char` |

#### HasColumnName

Default: The name of the property

Sets the name of the column that maps to the property.