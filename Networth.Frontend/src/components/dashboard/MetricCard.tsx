import { ReactNode } from "react";
import { cn } from "../../lib/utils";

interface MetricCardProps {
    label: string;
    value: string;
    change: string;
    changeType: "positive" | "negative" | "neutral";
    icon?: ReactNode;
    accentColor?: "blue" | "green" | "red" | "orange";
}

export function MetricCard({
    label,
    value,
    change,
    changeType,
    icon,
    accentColor = "blue",
}: MetricCardProps) {
    const colorClasses = {
        blue: "border-blue-500 bg-gradient-to-br from-blue-950/20 to-blue-900/10",
        green: "border-green-500 bg-gradient-to-br from-green-950/20 to-green-900/10",
        red: "border-red-500 bg-gradient-to-br from-red-950/20 to-red-900/10",
        orange: "border-orange-500 bg-gradient-to-br from-orange-950/20 to-orange-900/10",
    };

    const changeColorClasses = {
        positive: "text-green-400",
        negative: "text-red-400",
        neutral: "text-gray-400",
    };

    return (
        <div
            className={cn(
                "rounded-lg border p-6 backdrop-blur-sm",
                colorClasses[accentColor]
            )}
        >
            <div className="flex items-start justify-between mb-4">
                <div className="text-sm text-gray-400 font-medium">{label}</div>
                {icon && <div className="text-xl">{icon}</div>}
            </div>
            <div className="space-y-2">
                <div className="text-3xl font-bold text-white">{value}</div>
                <div className={cn("text-sm", changeColorClasses[changeType])}>
                    {change}
                </div>
            </div>
        </div>
    );
}
