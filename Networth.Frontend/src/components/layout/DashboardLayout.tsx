import { Outlet } from "react-router-dom";
import { DashboardSidebar } from "./DashboardSidebar";

export function DashboardLayout() {
    return (
        <div className="min-h-screen bg-gradient-to-b from-slate-950 to-slate-900">
            <DashboardSidebar />

            <div
                className="transition-all duration-300 ml-64"
            >
                {/* Header */}
                <header className="sticky top-0 z-40 backdrop-blur-xl bg-slate-950/80 border-b border-slate-800">
                    <div className="px-6 py-3 flex items-center justify-end h-16">
                        {/* Header content */}
                    </div>
                </header>

                {/* Main Content */}
                <main className="px-6 py-6 relative">
                    {/* Subtle decorative background */}
                    <div className="absolute top-0 left-0 w-64 h-64 bg-emerald-500/5 rounded-full blur-3xl -z-10"></div>
                    <div className="absolute bottom-0 right-0 w-48 h-48 bg-blue-500/5 rounded-full blur-3xl -z-10"></div>
                    <Outlet />
                </main>
            </div>
        </div>
    );
}
