import { useState } from "react";
import { Outlet } from "react-router-dom";
import { User } from "lucide-react";
import { DashboardSidebar } from "./DashboardSidebar";
import { Button } from "../ui/button";
import { SignOutButton } from "../ui/SignOutButton";
import { Avatar, AvatarFallback } from "../ui/avatar";

export function DashboardLayout() {
    const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);

    return (
        <div className="min-h-screen bg-gradient-to-b from-slate-950 to-slate-900">
            <DashboardSidebar
                isCollapsed={isSidebarCollapsed}
                onToggle={() => setIsSidebarCollapsed(!isSidebarCollapsed)}
            />

            <div
                className="transition-all duration-300"
                style={{
                    marginLeft: isSidebarCollapsed ? "80px" : "256px",
                }}
            >
                {/* Header */}
                <header className="sticky top-0 z-40 backdrop-blur-xl bg-slate-950/80 border-b border-slate-800">
                    <div className="px-8 py-4 flex items-center justify-end">
                        <div className="flex items-center gap-3">
                            <SignOutButton />
                            <Button
                                variant="ghost"
                                className="relative h-10 w-10 rounded-full p-0"
                            >
                                <Avatar className="h-10 w-10 border-2 border-emerald-500/30">
                                    <AvatarFallback>
                                        <User size={18} />
                                    </AvatarFallback>
                                </Avatar>
                            </Button>
                        </div>
                    </div>
                </header>

                {/* Main Content */}
                <main className="px-8 py-8 relative">
                    {/* Decorative background elements */}
                    <div className="absolute top-0 left-0 w-96 h-96 bg-emerald-500/5 rounded-full blur-3xl -z-10"></div>
                    <div className="absolute bottom-0 right-0 w-80 h-80 bg-blue-500/5 rounded-full blur-3xl -z-10"></div>
                    <Outlet />
                </main>
            </div>
        </div>
    );
}
