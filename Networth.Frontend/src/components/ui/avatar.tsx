import { cn } from "../../lib/utils";

type AvatarProps = React.HTMLAttributes<HTMLDivElement>;

export function Avatar({ className, ...props }: AvatarProps) {
    return (
        <div
            className={cn(
                "relative flex h-10 w-10 shrink-0 overflow-hidden rounded-full",
                className
            )}
            {...props}
        />
    );
}

type AvatarFallbackProps = React.HTMLAttributes<HTMLDivElement>;

export function AvatarFallback({ className, ...props }: AvatarFallbackProps) {
    return (
        <div
            className={cn(
                "flex h-full w-full items-center justify-center rounded-full bg-gradient-to-br from-emerald-500 to-blue-500 text-white font-semibold",
                className
            )}
            {...props}
        />
    );
}
