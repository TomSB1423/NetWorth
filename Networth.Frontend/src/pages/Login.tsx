import { Chrome } from "lucide-react";
import { useAuth } from "../contexts/AuthContext";

export default function Login() {
    const { login } = useAuth();

    // Google login via Firebase
    const loginWithGoogle = async () => {
        try {
            await login();
        } catch (error) {
            console.error("Login failed:", error);
        }
    };

    return (
        <div className="min-h-screen bg-slate-950 flex flex-col items-center justify-center px-6 relative overflow-hidden">
            {/* Background decorations */}
            <div className="absolute top-20 right-0 w-[500px] h-[500px] bg-gradient-to-br from-emerald-500/15 via-blue-500/8 to-transparent rounded-full blur-3xl pointer-events-none"></div>
            <div className="absolute bottom-0 left-0 w-[400px] h-[400px] bg-gradient-to-tr from-blue-500/10 to-transparent rounded-full blur-3xl pointer-events-none"></div>

            {/* Logo */}
            <div className="flex items-center gap-2 mb-6">
                <img
                    src="/networth-icon.svg"
                    alt="NetWorth"
                    className="w-8 h-8"
                />
                <span className="text-xl font-bold tracking-tight text-white">
                    NetWorth
                </span>
            </div>

            {/* Login Card */}
            <div className="w-full max-w-sm bg-slate-900/50 backdrop-blur-xl border border-slate-800 rounded-xl p-6">
                <div className="text-center mb-6">
                    <h1 className="text-xl font-bold text-white mb-1">
                        Welcome back
                    </h1>
                    <p className="text-slate-400 text-sm">
                        Sign in to continue tracking your net worth
                    </p>
                </div>

                <div className="space-y-2.5">
                    {/* Google Login */}
                    <button
                        onClick={loginWithGoogle}
                        className="w-full h-10 px-4 rounded-lg bg-white hover:bg-gray-50 text-gray-800 font-medium text-sm flex items-center justify-center gap-2.5 transition-all shadow-lg shadow-black/10"
                    >
                        <Chrome size={18} />
                        Continue with Google
                    </button>
                </div>

                {/* Terms */}
                <p className="text-xs text-slate-500 text-center mt-6 leading-relaxed">
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
