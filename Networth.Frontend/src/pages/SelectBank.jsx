import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { Search, ChevronLeft, Building2 } from "lucide-react";
import { api } from "../services/api";

export default function SelectBank() {
    const navigate = useNavigate();
    const [searchQuery, setSearchQuery] = useState("");

    const {
        data: institutions = [],
        isLoading,
        error,
    } = useQuery({
        queryKey: ["institutions"],
        queryFn: api.getInstitutions,
    });

    const filteredInstitutions = institutions.filter((inst) =>
        inst.name.toLowerCase().includes(searchQuery.toLowerCase())
    );

    const handleSelectInstitution = async (institution) => {
        try {
            const result = await api.linkInstitution(institution.id);
            if (result.authorizationLink) {
                window.location.assign(result.authorizationLink);
            } else {
                console.error("No authorization link returned");
                alert("Failed to start linking process. Please try again.");
            }
        } catch (err) {
            console.error("Failed to link account:", err);
            alert("Failed to link account. Please try again.");
        }
    };

    return (
        <div className="min-h-screen bg-slate-950">
            <header className="border-b border-slate-800 bg-slate-950/95 backdrop-blur sticky top-0 z-10">
                <div className="max-w-2xl mx-auto px-4 py-4 flex items-center gap-4">
                    <button
                        onClick={() => navigate(-1)}
                        className="p-2 hover:bg-slate-800 rounded-full text-gray-400 hover:text-white transition-colors"
                    >
                        <ChevronLeft size={24} />
                    </button>
                    <h1 className="text-xl font-semibold text-white">
                        Select your bank
                    </h1>
                </div>
            </header>

            <main className="max-w-2xl mx-auto px-4 py-8">
                <div className="relative mb-8">
                    <Search
                        className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400"
                        size={20}
                    />
                    <input
                        type="text"
                        placeholder="Search for your bank..."
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                        className="w-full bg-slate-900 border border-slate-800 rounded-xl py-4 pl-12 pr-4 text-white placeholder:text-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-500/50"
                    />
                </div>

                {isLoading ? (
                    <div className="text-center text-gray-400 py-12">
                        Loading institutions...
                    </div>
                ) : error ? (
                    <div className="text-center text-red-400 py-12">
                        Failed to load institutions. Please try again.
                    </div>
                ) : (
                    <div className="grid grid-cols-1 gap-2">
                        {filteredInstitutions.map((inst) => (
                            <button
                                key={inst.id}
                                onClick={() => handleSelectInstitution(inst)}
                                className="flex items-center gap-4 p-4 rounded-xl hover:bg-slate-900 transition-colors text-left group"
                            >
                                <div className="w-12 h-12 rounded-full bg-white p-2 flex items-center justify-center overflow-hidden">
                                    {inst.logoUrl ? (
                                        <img
                                            src={inst.logoUrl}
                                            alt={inst.name}
                                            className="w-full h-full object-contain"
                                        />
                                    ) : (
                                        <Building2 className="text-slate-900" />
                                    )}
                                </div>
                                <div className="flex-1">
                                    <h3 className="font-medium text-white group-hover:text-blue-400 transition-colors">
                                        {inst.name}
                                    </h3>
                                </div>
                            </button>
                        ))}
                        {filteredInstitutions.length === 0 && (
                            <div className="text-center text-gray-500 py-12">
                                No banks found matching "{searchQuery}"
                            </div>
                        )}
                    </div>
                )}
            </main>
        </div>
    );
}
