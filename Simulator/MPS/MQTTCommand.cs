namespace Simulator.MPS;

public class MQTTCommand {
    public enum COMMAND {
        NONE,
        GET_BASE,
        LIGHT,
        CAP_ACTION,
        MOUNT_RING,
        DELIVER,
        STORE,
        RETRIEVE,
        RELOCATE,
        MOVE_CONVEYOR,
        RESET,
    }
    public enum ARG1 {
        NONE,
        RED,
        BLACK,
        SILVER,
        GREEN,
        YELLOW,
        RESET,
        RETRIEVE,
        MOUNT,
        RING1,
        RING2,
        SLOT1,
        SLOT2,
        SLOT3,
        TARGET,
        TO_INPUT,
        TO_OUTPUT
    }
    public enum ARG2 {
        NONE,
        ON,
        OFF,
        BLINK,
        IN,
        MID,
        OUT,
        TARGET
    }

    public COMMAND command;
    public ARG1 arg1;
    public ARG2 arg2;
    public uint? arg1_shelf, arg1_slot, arg2_shelf, arg2_slot;

    public MQTTCommand(string command_string) {
        string[] command_parts = command_string.Split(" ");
        command = (COMMAND)Enum.Parse(typeof(COMMAND), command_parts[0]);
        if (command == COMMAND.STORE || command == COMMAND.RETRIEVE || command == COMMAND.RELOCATE) {
            arg1 = ARG1.TARGET;
            arg2 = ARG2.NONE;
            var parts = command_parts[1].Split(",");

            if (parts.Length > 0)
                uint.TryParse(parts[0], out uint arg1_shelf);
            if (parts.Length > 1)
                uint.TryParse(parts[1], out uint arg1_slot);

            if (command == COMMAND.RELOCATE && command_parts.Length > 2) {
                arg2 = ARG2.TARGET;
                parts = command_parts[2].Split(",");

                if (parts.Length > 0)
                    uint.TryParse(parts[0], out uint arg2_shelf);
                if (parts.Length > 1)
                    uint.TryParse(parts[1], out uint arg2_slot);
            }
            return;
        }

        arg1 = (command_parts.Length > 1) ? (ARG1)Enum.Parse(typeof(ARG1), command_parts[1]) : ARG1.NONE;
        arg2 = (command_parts.Length > 2) ? (ARG2)Enum.Parse(typeof(ARG2), command_parts[2]) : ARG2.NONE;
    }
    public MQTTCommand() {
        command = COMMAND.NONE;
        arg1 = ARG1.NONE;
        arg2 = ARG2.NONE;
    }

    public bool validate() {
        switch (command) {
            case (COMMAND.GET_BASE):
                if (arg1 == ARG1.SILVER || arg1 == ARG1.BLACK || arg1 == ARG1.RED)
                    return true;
                else
                    return false;
            case (COMMAND.LIGHT):
                switch (arg1) {
                    case ARG1.GREEN:
                    case ARG1.YELLOW:
                    case ARG1.RED:
                        if (arg2 == ARG2.ON || arg2 == ARG2.OFF || arg2 == ARG2.BLINK)
                            return true;
                        else
                            return false;
                    case ARG1.RESET:
                        return true;
                    default:
                        return false;
                }
            case (COMMAND.CAP_ACTION):
                if (arg1 == ARG1.RETRIEVE || arg1 == ARG1.RESET)
                    return true;
                else
                    return false;
            case (COMMAND.MOUNT_RING):
                if (arg1 == ARG1.RING1 || arg1 == ARG1.RING2)
                    return true;
                else
                    return false;
            case (COMMAND.DELIVER):
                if (arg1 == ARG1.SLOT1 || arg1 == ARG1.SLOT2 || arg1 == ARG1.SLOT3)
                    return true;
                else
                    return false;

            case (COMMAND.RETRIEVE):
            case (COMMAND.STORE):
                if (arg1 == ARG1.TARGET && arg1_shelf >= 0 && arg1_shelf <= 5 && arg1_slot >= 0 && arg1_slot <= 7)
                    return true;
                else
                    return false;

            case (COMMAND.RELOCATE):
                if (arg1 == ARG1.TARGET && arg2 == ARG2.TARGET &&
                    arg1_shelf >= 0 && arg1_shelf <= 5 && arg1_slot >= 0 && arg1_slot <= 7 &&
                    arg2_shelf >= 0 && arg2_shelf <= 5 && arg2_slot >= 0 && arg2_slot <= 7)
                    return true;
                else
                    return false;
            case (COMMAND.MOVE_CONVEYOR):
                if ((arg1 == ARG1.TO_INPUT || arg1 == ARG1.TO_OUTPUT) &&
                   (arg2 == ARG2.IN || arg2 == ARG2.MID || arg2 == ARG2.OUT))
                    return true;
                else
                    return false;
            case (COMMAND.RESET):
                return true;
        }
        return false;
    }
}
