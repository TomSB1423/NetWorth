import { useNavigate } from "react-router-dom";
import { LineChart } from "lucide-react";

export default function NotFound() {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen bg-slate-950 flex flex-col items-center justify-center text-center p-4">
            <div className="w-16 h-16 rounded-xl bg-gradient-to-br from-emerald-500 via-blue-500 to-emerald-500 flex items-center justify-center mb-6">
                <LineChart size={32} className="text-white" />
            </div>
            <h1 className="text-6xl font-bold text-white mb-4">404</h1>
            <p className="text-xl text-gray-400 mb-8">Page not found</p>
            <button
                onClick={() => navigate("/")}
                className="px-6 py-3 bg-emerald-500 hover:bg-emerald-600 text-white rounded-lg transition-colors font-medium"
            >
                Go Home
            </button>
        </div>
    );
}
