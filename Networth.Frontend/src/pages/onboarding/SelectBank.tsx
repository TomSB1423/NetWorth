import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import {
    Search,
    ChevronLeft,
    Building2,
    LineChart,
    LogOut,
} from "lucide-react";
import { api } from "../../services/api";
import { Institution } from "../../types";
import { useUser } from "../../contexts/UserContext";
import { useAuth } from "../../contexts/AuthContext";

export default function SelectBank() {
    const navigate = useNavigate();
    const [searchQuery, setSearchQuery] = useState("");
    const [isLinking, setIsLinking] = useState(false);
    const { logout } = useAuth();
    const {
        isProvisioned,
        isLoading: isUserLoading,
        error: userError,
    } = useUser();

    const {
        data: institutions = [],
        isLoading,
        error,
    } = useQuery({
        queryKey: ["institutions"],
        queryFn: api.getInstitutions,
        enabled: isProvisioned, // Only fetch institutions after user is provisioned
    });

    const filteredInstitutions = institutions.filter((inst: Institution) =>
        inst.name.toLowerCase().includes(searchQuery.toLowerCase())
    );

    const handleSelectInstitution = async (institution: Institution) => {
        setIsLinking(true);
        try {
            const result = await api.linkInstitution(institution.id);

            if (result.authorizationLink) {
                window.location.assign(result.authorizationLink);
            } else {
                console.error("No authorization link returned");
                alert("Failed to start linking process. Please try again.");
                setIsLinking(false);
            }
        } catch (err) {
            console.error("Failed to link account:", err);
            alert("Failed to link account. Please try again.");
            setIsLinking(false);
        }
    };

    // Show loading while user is being provisioned
    if (isUserLoading || !isProvisioned) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-slate-950">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-emerald-500"></div>
            </div>
        );
    }

    // Show error if user provisioning failed
    if (userError) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-slate-950">
                <div className="text-center">
                    <p className="text-red-500 mb-4">{userError}</p>
                    <button
                        onClick={() => window.location.reload()}
                        className="px-4 py-2 bg-emerald-600 text-white rounded-lg hover:bg-emerald-700"
                    >
                        Retry
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-slate-950">
            <header className="border-b border-slate-800 bg-slate-950/95 backdrop-blur sticky top-0 z-10">
                <div className="max-w-2xl mx-auto px-4 py-4 flex items-center justify-between">
                    <div className="flex items-center gap-4">
                        <button
                            onClick={() => navigate(-1)}
                            className="p-2 hover:bg-slate-800 rounded-full text-gray-400 hover:text-white transition-colors"
                        >
                            <ChevronLeft size={24} />
                        </button>
                        <div className="flex items-center gap-2">
                            <div className="w-7 h-7 rounded-lg bg-gradient-to-br from-emerald-500 via-blue-500 to-emerald-500 flex items-center justify-center">
                                <LineChart size={16} className="text-white" />
                            </div>
                            <h1 className="text-lg font-semibold text-white">
                                Select your bank
                            </h1>
                        </div>
                    </div>
                    <button
                        onClick={() => logout()}
                        className="flex items-center gap-2 px-3 py-1.5 rounded-lg border border-slate-700 text-gray-300 hover:text-white hover:border-slate-600 transition-colors"
                    >
                        <LogOut size={16} />
                        <span className="text-sm">Sign Out</span>
                    </button>
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
                        {filteredInstitutions.map((inst: Institution) => (
                            <button
                                key={inst.id}
                                onClick={() => handleSelectInstitution(inst)}
                                disabled={isLinking}
                                className="flex items-center gap-4 p-4 rounded-xl hover:bg-slate-900 transition-colors text-left group disabled:opacity-50 disabled:cursor-not-allowed"
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

            {/* Linking in Progress Overlay */}
            {isLinking && (
                <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50">
                    <div className="text-center space-y-4">
                        <div className="w-12 h-12 border-4 border-emerald-500 border-t-transparent rounded-full animate-spin mx-auto"></div>
                        <p className="text-white font-medium">
                            Connecting to bank...
                        </p>
                    </div>
                </div>
            )}
        </div>
    );
}
