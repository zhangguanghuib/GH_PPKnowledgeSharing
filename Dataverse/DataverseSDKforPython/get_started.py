from azure.identity import InteractiveBrowserCredential
from PowerPlatform.Dataverse.client import DataverseClient


DATAVERSE_URL = "https://org5efadec2.crm.dynamics.com"


def main() -> None:
    """Connect to Dataverse and verify access with a read-only query."""
    credential = InteractiveBrowserCredential()

    with DataverseClient(DATAVERSE_URL, credential) as client:
        users = client.records.list(
            "systemuser",
            select=["systemuserid", "fullname"],
            top=1,
        )

        if not users:
            raise RuntimeError("Connected to Dataverse, but no enabled user was returned.")

        user = users[0]
        print(f"Connected to Dataverse: {DATAVERSE_URL}")
        print(f"Sample system user: {user.get('fullname', '<name unavailable>')}")
        print(f"Sample system user ID: {user['systemuserid']}")


if __name__ == "__main__":
    main()