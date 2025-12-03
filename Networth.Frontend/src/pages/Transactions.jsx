import React from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { ChevronLeft, ArrowUpRight, ArrowDownLeft } from "lucide-react";
import { api } from "../services/api";

export default function Transactions() {
    const { id } = useParams();
    const navigate = useNavigate();

    // Default to last 90 days
    const dateTo = new Date().toISOString().split('T')[0];
    const dateFrom = new Date(Date.now() - 90 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];

    const { data: transactions = [], isLoading, error } = useQuery({
        queryKey: ["transactions", id, dateFrom, dateTo],
        queryFn: () => api.getTransactions(id, dateFrom, dateTo),
    });

    const formatCurrency = (amount, currency) => {
        return new Intl.NumberFormat("en-GB", {
            style: "currency",
            currency: currency,
        }).format(amount);
    };

    const formatDate = (dateString) => {
        return new Date(dateString).toLocaleDateString("en-GB", {
            day: "numeric",
            month: "short",
            year: "numeric",
        });
    };

    const getTransactionDescription = (tx) => {
        return tx.remittanceInformationUnstructured || 
               tx.creditorName || 
               tx.debtorName || 
               tx.proprietaryBankTransactionCode || 
               "Transaction";
    };

    return (
        <div className="min-h-screen bg-slate-950 text-white">
            <header className="border-b border-slate-800 bg-slate-950/95 backdrop-blur sticky top-0 z-10">
                <div className="max-w-4xl mx-auto px-4 py-4 flex items-center gap-4">
                    <button
                        onClick={() => navigate(-1)}
                        className="p-2 hover:bg-slate-800 rounded-full text-gray-400 hover:text-white transition-colors"
                    >
                        <ChevronLeft size={24} />
                    </button>
                    <h1 className="text-xl font-semibold">Transactions</h1>
                </div>
            </header>

            <main className="max-w-4xl mx-auto px-4 py-8">
                {isLoading ? (
                    <div className="text-center text-gray-400 py-12">
                        Loading transactions...
                    </div>
                ) : error ? (
                    <div className="text-center text-red-400 py-12">
                        Failed to load transactions.
                    </div>
                ) : (
                    <div className="space-y-4">
                        {transactions.length === 0 ? (
                            <div className="text-center text-gray-500 py-12">
                                No transactions found for the last 90 days.
                            </div>
                        ) : (
                            transactions.map((tx) => (
                                <div
                                    key={tx.id}
                                    className="bg-slate-900/50 rounded-xl p-4 flex items-center justify-between hover:bg-slate-900 transition-colors"
                                >
                                    <div className="flex items-center gap-4">
                                        <div className={`p-2 rounded-full ${tx.amount > 0 ? 'bg-green-500/10 text-green-500' : 'bg-white/5 text-white'}`}>
                                            {tx.amount > 0 ? <ArrowDownLeft size={20} /> : <ArrowUpRight size={20} />}
                                        </div>
                                        <div>
                                            <div className="font-medium text-white">
                                                {getTransactionDescription(tx)}
                                            </div>
                                            <div className="text-sm text-gray-400">
                                                {formatDate(tx.bookingDate || tx.valueDate)}
                                            </div>
                                        </div>
                                    </div>
                                    <div className={`font-semibold ${tx.amount > 0 ? 'text-green-400' : 'text-white'}`}>
                                        {formatCurrency(tx.amount, tx.currency)}
                                    </div>
                                </div>
                            ))
                        )}
                    </div>
                )}
            </main>
        </div>
    );
}
