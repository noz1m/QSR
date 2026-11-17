namespace QSR.Domain.Enums;

public enum StockMovementType
{
    Receipt = 1,

    Reservation = 2,
    ReservationRelease = 3,

    SaleDeduction = 4,
    Return = 5,

    AdjustmentIncrease = 6,
    AdjustmentDecrease = 7,

    Waste = 8
}