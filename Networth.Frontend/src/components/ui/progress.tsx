import { cn } from "../../lib/utils";

interface ProgressProps {
    value: number;
    className?: string;
    indicatorClassName?: string;
}

export function Progress({
    value,
    className,
    indicatorClassName,
}: ProgressProps) {
    return (
        <div
            className={cn(
                "relative h-2 w-full overflow-hidden rounded-full bg-slate-800",
                className
            )}
        >
            <div
                className={cn(
                    "h-full rounded-full bg-emerald-500 transition-all",
                    indicatorClassName
                )}
                style={{ width: `${Math.min(100, Math.max(0, value))}%` }}
            />
        </div>
    );
}
