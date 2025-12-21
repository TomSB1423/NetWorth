import { NavLink } from "react-router-dom";
import { useMsal } from "@azure/msal-react";
import {
    LayoutDashboard,
    Wallet,
    Receipt,
    User,
} from "lucide-react";
import { SignOutButton } from "../ui/SignOutButton";
import { Avatar, AvatarFallback } from "../ui/avatar";

export function DashboardSidebar() {
    const { accounts } = useMsal();
    const user = accounts[0];

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
        <aside className="fixed left-0 top-0 h-screen w-64 bg-slate-950 border-r border-slate-800 flex flex-col z-50">
            {/* Logo */}
            <div className="h-16 flex items-center px-6 border-b border-slate-800/50">
                <div className="flex items-center gap-3">
                    <img src="/networth-icon.svg" alt="NetWorth" className="w-8 h-8" />
                    <span className="text-lg font-semibold text-white tracking-tight">
                        NetWorth
                    </span>
                </div>
            </div>

            {/* Navigation */}
            <nav className="flex-1 p-4 space-y-1">
                <div className="text-xs font-semibold text-slate-500 uppercase tracking-wider mb-4 px-2">
                    Menu
                </div>
                {menuItems.map((item) => {
                    const Icon = item.icon;
                    return (
                        <NavLink
                            key={item.id}
                            to={item.path}
                            end={item.path === "/dashboard"}
                            className={({ isActive }) =>
                                `flex items-center gap-3 px-3 py-2.5 rounded-lg transition-all duration-200 font-medium text-sm group ${
                                    isActive
                                        ? "bg-emerald-500/10 text-emerald-400"
                                        : "text-slate-400 hover:text-slate-100 hover:bg-slate-800/50"
                                }`
                            }
                        >
                            <Icon size={18} />
                            <span>{item.label}</span>
                        </NavLink>
                    );
                })}
            </nav>
            
            {/* User Profile */}
            <div className="p-4 border-t border-slate-800/50">
                <div className="flex items-center gap-3 px-2 mb-3">
                    <Avatar className="h-9 w-9 border border-slate-700">
                        <AvatarFallback className="bg-slate-800 text-slate-400">
                            <User size={16} />
                        </AvatarFallback>
                    </Avatar>
                    <div className="flex-1 min-w-0">
                        <p className="text-sm font-medium text-slate-200 truncate">
                            {user?.name || "User Account"}
                        </p>
                        <p className="text-xs text-slate-500 truncate">
                            {user?.username || ""}
                        </p>
                    </div>
                </div>
                <SignOutButton className="w-full justify-start pl-2 text-slate-400 hover:text-slate-100 hover:bg-slate-800/50" />
            </div>
        </aside>
    );
}
