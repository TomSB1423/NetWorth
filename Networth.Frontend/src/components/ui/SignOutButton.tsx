import { LogOut } from "lucide-react";
import { useNavigate } from "react-router-dom";
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
    const navigate = useNavigate();

    const handleSignOut = async () => {
        try {
            await logout();
            // Navigate to landing page after logout
            navigate("/", { replace: true });
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
