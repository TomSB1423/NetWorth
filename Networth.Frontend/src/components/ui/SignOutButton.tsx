import { LogOut } from "lucide-react";
import { useMsal } from "@azure/msal-react";
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
    const { instance, accounts } = useMsal();

    const handleSignOut = async () => {
        try {
            // Get the active account to avoid account picker prompt
            const activeAccount = instance.getActiveAccount() || accounts[0];
            
            await instance.logoutRedirect({
                account: activeAccount,
                postLogoutRedirectUri: window.location.origin,
            });
        } catch (error) {
            console.error("Logout failed:", error);
        }
    };

    return (
        <Button
            variant="ghost"
            onClick={handleSignOut}
            className={className}
        >
            <LogOut size={18} />
            {showLabel && <span className="ml-2">Sign Out</span>}
        </Button>
    );
}
