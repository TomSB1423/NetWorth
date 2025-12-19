import { NavLink } from "react-router-dom";
import {
    LayoutDashboard,
    Wallet,
    ChevronLeft,
    ChevronRight,
    LineChart,
    Receipt,
} from "lucide-react";
import { Button } from "../ui/button";

interface DashboardSidebarProps {
    isCollapsed: boolean;
    onToggle: () => void;
}

export function DashboardSidebar({
    isCollapsed,
    onToggle,
}: DashboardSidebarProps) {
    const menuItems = [
        {
            id: "overview",
            label: "Overview",
            icon: LayoutDashboard,
            path: "/dashboard",
        },
        {
            id: "transactions",
            label: "Transactions",
            icon: Receipt,
            path: "/dashboard/transactions",
        },
        {
            id: "accounts",
            label: "Accounts",
            icon: Wallet,
            path: "/dashboard/accounts",
        },
    ];

    return (
        <aside
            className={`fixed left-0 top-0 h-screen bg-slate-900/95 backdrop-blur border-r border-slate-800 flex flex-col z-50 transition-all duration-300 ${
                isCollapsed ? "w-20" : "w-64"
            }`}
        >
            {/* Logo */}
            <div
                className={`p-4 flex items-center ${
                    isCollapsed ? "justify-center" : "justify-between"
                } border-b border-slate-800`}
            >
                {!isCollapsed && (
                    <div className="flex items-center gap-3">
                        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-emerald-500 via-blue-500 to-emerald-500 flex items-center justify-center">
                            <LineChart size={22} className="text-white" />
                        </div>
                        <span className="text-xl font-bold text-white">
                            NetWorth
                        </span>
                    </div>
                )}
                {isCollapsed && (
                    <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-emerald-500 via-blue-500 to-emerald-500 flex items-center justify-center">
                        <LineChart size={22} className="text-white" />
                    </div>
                )}
            </div>

            {/* Navigation */}
            <nav className={`flex-1 ${isCollapsed ? "p-2" : "p-4"} space-y-1`}>
                {menuItems.map((item) => {
                    const Icon = item.icon;
                    return (
                        <NavLink
                            key={item.id}
                            to={item.path}
                            end={item.path === "/dashboard"}
                            className={({ isActive }) =>
                                `flex items-center ${
                                    isCollapsed
                                        ? "justify-center"
                                        : "justify-start"
                                } gap-3 px-3 py-3 rounded-xl transition-all duration-200 font-medium text-sm ${
                                    isActive
                                        ? "bg-emerald-500/20 text-emerald-400 border-l-4 border-emerald-500"
                                        : "text-gray-400 hover:text-white hover:bg-slate-800"
                                } ${isCollapsed ? "h-12" : "h-11"}`
                            }
                        >
                            <Icon size={20} />
                            {!isCollapsed && <span>{item.label}</span>}
                        </NavLink>
                    );
                })}
            </nav>

            {/* Collapse Button */}
            <div
                className={`${
                    isCollapsed ? "p-2" : "p-4"
                } border-t border-slate-800`}
            >
                <Button
                    variant="outline"
                    onClick={onToggle}
                    className={`${
                        isCollapsed ? "w-full justify-center" : "w-full"
                    }`}
                >
                    {isCollapsed ? (
                        <ChevronRight size={18} />
                    ) : (
                        <>
                            <ChevronLeft size={18} />
                            <span className="ml-2">Collapse</span>
                        </>
                    )}
                </Button>
            </div>
        </aside>
    );
}
