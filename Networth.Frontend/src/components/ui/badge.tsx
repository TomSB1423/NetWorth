import { cn } from "../../lib/utils";

interface BadgeProps extends React.HTMLAttributes<HTMLSpanElement> {
    variant?: "default" | "secondary" | "success" | "destructive" | "outline";
}

export function Badge({
    className,
    variant = "default",
    ...props
}: BadgeProps) {
    const variants = {
        default: "bg-emerald-500/20 text-emerald-400 border-emerald-500/30",
        secondary: "bg-slate-700 text-gray-300 border-slate-600",
        success: "bg-green-500/20 text-green-400 border-green-500/30",
        destructive: "bg-red-500/20 text-red-400 border-red-500/30",
        outline: "border-slate-700 text-gray-300",
    };

    return (
        <span
            className={cn(
                "inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-medium",
                variants[variant],
                className
            )}
            {...props}
        />
    );
}
