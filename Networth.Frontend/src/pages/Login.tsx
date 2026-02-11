import { Chrome } from "lucide-react";
import { useNavigate, useLocation, Link } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

export default function Login() {
    const { login } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();

    // Check if this is signup or signin based on route
    const isSignUp = location.pathname === "/signup";

    // Google login via Firebase (or mock login in dev mode)
    const loginWithGoogle = async () => {
        try {
            await login();
            // Navigate to dashboard after successful login
            navigate("/dashboard", { replace: true });
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
            <Link to="/" className="flex items-center gap-2 mb-8">
                <img
                    src="/networth-icon.svg"
                    alt="NetWorth"
                    className="w-10 h-10"
                />
                <span className="text-2xl font-bold tracking-tight text-white">
                    NetWorth
                </span>
            </Link>

            {/* Login Card */}
            <div className="w-full max-w-sm bg-slate-900/50 backdrop-blur-xl border border-slate-800 rounded-xl p-8">
                <div className="text-center mb-8">
                    <h1 className="text-2xl font-bold text-white mb-2">
                        {isSignUp ? "Create your account" : "Welcome back"}
                    </h1>
                    <p className="text-slate-400 text-sm">
                        {isSignUp
                            ? "Start tracking your net worth today"
                            : "Sign in to continue to your dashboard"}
                    </p>
                </div>

                <div className="space-y-3">
                    {/* Google Login */}
                    <button
                        onClick={loginWithGoogle}
                        className="w-full h-11 px-4 rounded-lg bg-white hover:bg-gray-50 text-gray-800 font-medium text-sm flex items-center justify-center gap-2.5 transition-all shadow-lg shadow-black/10"
                    >
                        <Chrome size={18} />
                        {isSignUp ? "Sign up with Google" : "Sign in with Google"}
                    </button>
                </div>

                {/* Divider */}
                <div className="relative my-6">
                    <div className="absolute inset-0 flex items-center">
                        <div className="w-full border-t border-slate-700"></div>
                    </div>
                    <div className="relative flex justify-center text-xs">
                        <span className="bg-slate-900/50 px-2 text-slate-500">
                            {isSignUp ? "Already have an account?" : "New to NetWorth?"}
                        </span>
                    </div>
                </div>

                {/* Switch between Sign In / Sign Up */}
                <Link
                    to={isSignUp ? "/login" : "/signup"}
                    className="block w-full h-11 px-4 rounded-lg border border-slate-700 hover:border-slate-600 hover:bg-slate-800/50 text-white font-medium text-sm text-center leading-[2.75rem] transition-all"
                >
                    {isSignUp ? "Sign in instead" : "Create an account"}
                </Link>

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
            <Link
                to="/"
                className="mt-6 text-sm text-slate-400 hover:text-emerald-400 transition-colors"
            >
                ← Back to home
            </Link>
        </div>
    );
}
