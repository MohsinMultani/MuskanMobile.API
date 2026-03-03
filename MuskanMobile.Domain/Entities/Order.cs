using MuskanMobile.Domain.Common;
using System;

namespace MuskanMobile.Domain.Entities;

public class Order : BaseEntity
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime OrderDate { get; set; }
}
