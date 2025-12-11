
import { useNavigate } from "react-router-dom";

export default function NotFound() {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen bg-slate-950 flex flex-col items-center justify-center text-center p-4">
            <h1 className="text-6xl font-bold text-white mb-4">404</h1>
            <p className="text-xl text-gray-400 mb-8">Page not found</p>
            <button
                onClick={() => navigate("/")}
                className="px-6 py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors"
            >
                Go Home
            </button>
        </div>
    );
}
