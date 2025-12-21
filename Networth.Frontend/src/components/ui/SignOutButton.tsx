import { LogOut } from "lucide-react";
import { useAuth } from "../../contexts/AuthContext";
import { Button } from "./button";

interface SignOutButtonProps {
    /** Whether to show the text label alongside the icon */
    showLabel?: boolean;
    /** Additional CSS classes */
    className?: string;
}

export function SignOutButton({
    showLabel = true,
    className,
}: SignOutButtonProps) {
    const { logout } = useAuth();

    const handleSignOut = async () => {
        try {
            await logout();
        } catch (error) {
            console.error("Logout failed:", error);
        }
    };

    return (
        <Button variant="ghost" onClick={handleSignOut} className={className}>
            <LogOut size={18} />
            {showLabel && <span className="ml-2">Sign Out</span>}
        </Button>
    );
}
