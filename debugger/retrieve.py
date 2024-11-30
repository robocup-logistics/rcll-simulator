#!/bin/python3
import sys
import os
import json
import uuid
from pymongo import MongoClient
from datetime import datetime

def main():
    # Get the MongoDB connection string from the first argument or default to localhost
    connection_string = sys.argv[1] if len(sys.argv) > 1 else "mongodb://localhost:27017"

    try:
        # Connect to MongoDB
        client = MongoClient(connection_string)
        
        # Retrieve all entries from the 'rcll' database and 'game_report' collection
        db = client['rcll']
        collection = db['game_report']
        entries = list(collection.find())

        if not entries:
            print("No entries found in rcll/game_report.")
            return

        # Display all entries in an ordered table format and prompt user to select one
        print("Select an entry by typing the number and pressing Enter:")
        print(f"{'No.':<5}{'(Time Ago)':<10}{'Start Time':<30}{'Length (min:sec)':<30}")
        print("=" * 75)
        now = datetime.now()
        for idx, entry in enumerate(entries):
            start_time = entry.get("start_time")
            end_time = entry.get("end_time")
            if isinstance(start_time, datetime) and isinstance(end_time, datetime):
                length_seconds = int((end_time - start_time).total_seconds())
                length_str = f"{length_seconds // 60}:{length_seconds % 60:02d}"
                time_ago = now - start_time
                if time_ago.days > 0:
                    time_ago_str = f"{time_ago.days}d ago"
                elif time_ago.seconds >= 3600:
                    time_ago_str = f"{time_ago.seconds // 3600}h ago"
                else:
                    time_ago_str = f"{time_ago.seconds // 60}m ago"
                entry_str = f"{idx + 1:<5}{time_ago_str:<10}{start_time.strftime('%Y-%m-%d %H:%M:%S'):<30}{length_str:<30}"
            else:
                entry_str = f"{idx + 1:<5}{'Invalid':<10}{'Invalid entry':<30}{'Invalid entry':<30}"
            print(entry_str)

        # Get user input
        while True:
            try:
                selection = int(input("Enter the number of the entry you want to select: "))
                if 1 <= selection <= len(entries):
                    selected_entry = entries[selection - 1]
                    print(f"You selected entry number {selection}")
                    break
                else:
                    print("Invalid selection. Please enter a number between 1 and", len(entries))
            except ValueError:
                print("Invalid input. Please enter a valid number.")

        # Generate a random UUID and update the selected entry
        report_name = str(uuid.uuid4())
        selected_entry["report_name"] = report_name
        update_result = collection.update_one({"_id": selected_entry["_id"]}, {"$set": {"report_name": report_name}})

        # Verify that the update was successful
        if update_result.matched_count == 0:
            print("Failed to update the report_name in the MongoDB server.")
            return
        else:
            print("Successfully updated the report_name in the MongoDB server.")


        # Ask for filename to save the selected entry
        default_filename = selected_entry.get("start_time").strftime('%Y-%m-%d_%H-%M-%S') if selected_entry.get("start_time") else "default_report"
        filename = input(f"Enter the filename to save the entry (default: {default_filename}): ") or default_filename
        
        # Ensure the 'game_logs' directory exists
        os.makedirs("game_logs", exist_ok=True)
        filepath = os.path.join("game_logs", filename + ".json")

        # Save the entire selected entry to the file in JSON format
        with open(filepath, "w") as file:
            json.dump(selected_entry, file, indent=4, default=str)
        print(f"Selected entry saved to {filepath}")

        # Create a symlink to the latest file
        latest_symlink = "latest"
        if os.path.islink(latest_symlink) or os.path.exists(latest_symlink):
            os.remove(latest_symlink)
        os.symlink(filepath, latest_symlink)
        print(f"Symlink created at {latest_symlink}")

    except Exception as e:
        print(f"Failed to connect to MongoDB or retrieve data: {e}")

if __name__ == "__main__":
    main()
