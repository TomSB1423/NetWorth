import React, { useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { useQueryClient } from "@tanstack/react-query";
import { api } from "../services/api";

export default function LinkCallback() {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const queryClient = useQueryClient();
    const ref = searchParams.get("ref"); // GoCardless usually sends a ref or similar
    const institutionId = searchParams.get("institutionId");

    useEffect(() => {
        const handleCallback = async () => {
            try {
                if (institutionId) {
                    // Sync the institution to discover accounts
                    await api.syncInstitution(institutionId);
                }

                // Invalidate accounts query to refresh the list
                await queryClient.invalidateQueries({ queryKey: ["accounts"] });
                await queryClient.invalidateQueries({ queryKey: ["balances"] });

                // Redirect to dashboard
                navigate("/");
            } catch (error) {
                console.error("Failed to complete linking:", error);
                // Optionally navigate to error page or show error
                navigate("/");
            }
        };

        handleCallback();
    }, [navigate, queryClient, ref, institutionId]);

    return (
        <div className="min-h-screen bg-slate-950 flex items-center justify-center">
            <div className="text-center space-y-4">
                <div className="w-16 h-16 border-4 border-blue-500 border-t-transparent rounded-full animate-spin mx-auto"></div>
                <h2 className="text-xl font-semibold text-white">
                    Completing account setup...
                </h2>
                <p className="text-gray-400">
                    Please wait while we secure your connection.
                </p>
            </div>
        </div>
    );
}
