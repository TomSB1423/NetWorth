
import { useNavigate } from "react-router-dom";

export default function NameAccount() {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen bg-slate-950 flex items-center justify-center">
            <div className="text-center space-y-4">
                <h1 className="text-2xl font-bold text-white">Name Account</h1>
                <p className="text-gray-400">This feature is coming soon.</p>
                <button
                    onClick={() => navigate("/")}
                    className="text-blue-400 hover:text-blue-300"
                >
                    Go back home
                </button>
            </div>
        </div>
    );
}
