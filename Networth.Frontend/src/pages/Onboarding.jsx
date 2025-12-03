import React from "react";
import { useNavigate } from "react-router-dom";
import { ArrowRight, Shield, Lock, Globe } from "lucide-react";

export default function Onboarding() {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen bg-slate-950 flex flex-col items-center justify-center p-4">
            <div className="max-w-md w-full space-y-8 text-center">
                <div className="space-y-2">
                    <h1 className="text-4xl font-bold text-white tracking-tight">
                        Welcome to NetWorth
                    </h1>
                    <p className="text-gray-400 text-lg">
                        Your complete financial picture in one place. Secure,
                        simple, and smart.
                    </p>
                </div>

                <div className="grid grid-cols-1 gap-4 py-8">
                    <div className="flex items-center gap-4 p-4 rounded-lg bg-slate-900/50 border border-slate-800">
                        <div className="p-2 bg-blue-500/10 rounded-lg">
                            <Globe className="w-6 h-6 text-blue-400" />
                        </div>
                        <div className="text-left">
                            <h3 className="font-semibold text-white">
                                Connect Everything
                            </h3>
                            <p className="text-sm text-gray-400">
                                Link all your bank accounts and investments
                            </p>
                        </div>
                    </div>

                    <div className="flex items-center gap-4 p-4 rounded-lg bg-slate-900/50 border border-slate-800">
                        <div className="p-2 bg-green-500/10 rounded-lg">
                            <Shield className="w-6 h-6 text-green-400" />
                        </div>
                        <div className="text-left">
                            <h3 className="font-semibold text-white">
                                Bank-Grade Security
                            </h3>
                            <p className="text-sm text-gray-400">
                                Your data is encrypted and protected
                            </p>
                        </div>
                    </div>

                    <div className="flex items-center gap-4 p-4 rounded-lg bg-slate-900/50 border border-slate-800">
                        <div className="p-2 bg-purple-500/10 rounded-lg">
                            <Lock className="w-6 h-6 text-purple-400" />
                        </div>
                        <div className="text-left">
                            <h3 className="font-semibold text-white">
                                Private by Design
                            </h3>
                            <p className="text-sm text-gray-400">
                                We never sell your personal data
                            </p>
                        </div>
                    </div>
                </div>

                <button
                    onClick={() => navigate("/select-bank")}
                    className="w-full flex items-center justify-center gap-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold py-4 px-8 rounded-lg transition-all transform hover:scale-[1.02]"
                >
                    Get Started
                    <ArrowRight className="w-5 h-5" />
                </button>
            </div>
        </div>
    );
}
