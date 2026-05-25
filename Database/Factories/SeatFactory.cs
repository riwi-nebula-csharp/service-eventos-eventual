using service_eventos_eventual.Models;

namespace service_eventos_eventual.Database.Factories;

public class SeatFactory
{
    private static Seat Create(char rowName, int seatNumber)
    {
        int rowNumber = rowName - 'A' + 1; // A=1, B=2... J=10
        int seatOrder = (rowNumber - 1) * 10 + seatNumber; // Global order 1-100

        return new Seat
        {
            RowName = rowName,
            RowNumber = rowNumber,
            SeatNumber = seatNumber,
            SeatOrder = seatOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static List<Seat> CreateMany()
    {
        var seats = new List<Seat>();
        var rows = "ABCDEFGHIJ".ToCharArray();

        foreach (var row in rows)
            for (int seatNumber = 1; seatNumber <= 10; seatNumber++)
                seats.Add(Create(row, seatNumber));
        return seats;
    }
}