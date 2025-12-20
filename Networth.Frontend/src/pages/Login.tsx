import { LineChart, Mail, Chrome } from "lucide-react";
import { useMsal } from "@azure/msal-react";
import { loginRequest } from "../config/authConfig";

export default function Login() {
    const { instance } = useMsal();

    // Base auth request shared by all login methods
    const baseAuthRequest = {
        ...loginRequest,
    };

    // Email/password login - uses the default provider selection in the user flow
    const loginWithEmail = () => {
        instance.loginRedirect({
            ...baseAuthRequest,
        });
    };

    // Google login - deep-links directly to Google IdP, skipping provider picker
    const loginWithGoogle = () => {
        instance.loginRedirect({
            ...baseAuthRequest,
            extraQueryParameters: {
                idp: "google.com",
            },
        });
    };

    return (
        <div className="min-h-screen bg-slate-950 flex flex-col items-center justify-center px-6 relative overflow-hidden">
            {/* Background decorations */}
            <div className="absolute top-20 right-0 w-[500px] h-[500px] bg-gradient-to-br from-emerald-500/15 via-blue-500/8 to-transparent rounded-full blur-3xl pointer-events-none"></div>
            <div className="absolute bottom-0 left-0 w-[400px] h-[400px] bg-gradient-to-tr from-blue-500/10 to-transparent rounded-full blur-3xl pointer-events-none"></div>

            {/* Logo */}
            <div className="flex items-center gap-2.5 mb-8">
                <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-emerald-500 via-blue-500 to-emerald-500 flex items-center justify-center">
                    <LineChart size={24} className="text-white" />
                </div>
                <span className="text-2xl font-black tracking-tight text-white">
                    NetWorth
                </span>
            </div>

            {/* Login Card */}
            <div className="w-full max-w-md bg-slate-900/50 backdrop-blur-xl border border-slate-800 rounded-2xl p-8">
                <div className="text-center mb-8">
                    <h1 className="text-2xl font-bold text-white mb-2">
                        Welcome back
                    </h1>
                    <p className="text-slate-400 text-sm">
                        Sign in to continue tracking your net worth
                    </p>
                </div>

                <div className="space-y-3">
                    {/* Google Login */}
                    <button
                        onClick={loginWithGoogle}
                        className="w-full h-12 px-4 rounded-xl bg-white hover:bg-gray-50 text-gray-800 font-semibold flex items-center justify-center gap-3 transition-all shadow-lg shadow-black/10"
                    >
                        <Chrome size={20} />
                        Continue with Google
                    </button>

                    {/* Divider */}
                    <div className="flex items-center gap-4 my-6">
                        <div className="flex-1 h-px bg-slate-700"></div>
                        <span className="text-xs text-slate-500 font-medium">
                            OR
                        </span>
                        <div className="flex-1 h-px bg-slate-700"></div>
                    </div>

                    {/* Email Login */}
                    <button
                        onClick={loginWithEmail}
                        className="w-full h-12 px-4 rounded-xl bg-slate-800 hover:bg-slate-700 text-white font-semibold flex items-center justify-center gap-3 transition-all border border-slate-700"
                    >
                        <Mail size={20} />
                        Continue with Email
                    </button>
                </div>

                {/* Terms */}
                <p className="text-xs text-slate-500 text-center mt-8 leading-relaxed">
                    By continuing, you agree to our{" "}
                    <a
                        href="/terms"
                        className="text-emerald-400 hover:text-emerald-300 underline"
                    >
                        Terms of Service
                    </a>{" "}
                    and{" "}
                    <a
                        href="/privacy"
                        className="text-emerald-400 hover:text-emerald-300 underline"
                    >
                        Privacy Policy
                    </a>
                    .
                </p>
            </div>

            {/* Back to home */}
            <a
                href="/"
                className="mt-6 text-sm text-slate-400 hover:text-emerald-400 transition-colors"
            >
                ← Back to home
            </a>
        </div>
    );
}
