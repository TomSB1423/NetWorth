using Networth.Infrastructure.Data.Entities;
using InfraTransaction = Networth.Infrastructure.Data.Entities.Transaction;

namespace Networth.Infrastructure.Data.Queries;

public static class NetWorthHistoryQuery
{
    public static string GetDailyBalancesCte(string userIdParam)
    {
        return $$"""
            DailyBalances AS (
                SELECT DISTINCT ON ("{{nameof(InfraTransaction.AccountId)}}", "Date")
                    "{{nameof(InfraTransaction.AccountId)}}",
                    COALESCE("{{nameof(InfraTransaction.BookingDate)}}", "{{nameof(InfraTransaction.ValueDate)}}", "{{nameof(InfraTransaction.ImportedAt)}}")::date AS "Date",
                    "{{nameof(InfraTransaction.RunningBalance)}}"
                FROM "Transactions"
                WHERE "{{nameof(InfraTransaction.UserId)}}" = {{userIdParam}}
                  AND "{{nameof(InfraTransaction.RunningBalance)}}" IS NOT NULL
                ORDER BY "{{nameof(InfraTransaction.AccountId)}}", "Date", COALESCE("{{nameof(InfraTransaction.BookingDate)}}", "{{nameof(InfraTransaction.ValueDate)}}", "{{nameof(InfraTransaction.ImportedAt)}}") DESC, "{{nameof(InfraTransaction.TransactionId)}}" DESC
            )
            """;
    }

    public static string GetAccountDeltasCte()
    {
        return $$"""
            AccountDeltas AS (
                SELECT
                    "Date",
                    "{{nameof(InfraTransaction.RunningBalance)}}" - LAG("{{nameof(InfraTransaction.RunningBalance)}}", 1, 0) OVER (PARTITION BY "{{nameof(InfraTransaction.AccountId)}}" ORDER BY "Date") as "Delta"
                FROM DailyBalances
            )
            """;
    }

    public static string GetDailyNetChangeCte()
    {
        return """
            DailyNetChange AS (
                SELECT
                    "Date",
                    SUM("Delta") as "TotalChange"
                FROM AccountDeltas
                GROUP BY "Date"
            )
            """;
    }

    public static string GetFinalSelect()
    {
        return """
            SELECT
                CAST("Date" AS timestamp) as "Date",
                SUM("TotalChange") OVER (ORDER BY "Date") as "Amount"
            FROM DailyNetChange
            ORDER BY "Date"
            """;
    }

    public static string GetFullQuery(string userIdParam)
    {
        return $$"""
            WITH {{GetDailyBalancesCte(userIdParam)}},
            {{GetAccountDeltasCte()}},
            {{GetDailyNetChangeCte()}}
            {{GetFinalSelect()}}
            """;
    }
}
