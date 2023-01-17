import useAuth from "@hooks/useAuth";
import {
  ActionIcon,
  Box,
  Button,
  Card,
  Flex,
  Group,
  Image,
  Modal,
  Text,
  TextInput,
} from "@mantine/core";
import { DateRangePicker } from "@mantine/dates";
import { useToggle } from "@mantine/hooks";
import { IconDownload, IconEye } from "@tabler/icons";
import downloadImage from "@utils/downloadImage";
import { UserRole } from "@utils/enums";
import { useState } from "react";

const MyTrainingExternal = ({ isAdmin }: { isAdmin?: boolean }) => {
  const [showConfirmation, setShowConfirmation] = useToggle();
  const [value, setValue] = useState<[Date, Date]>([
    new Date(2021, 11, 1),
    new Date(2021, 11, 5),
  ]);

  const auth = useAuth();
  return (
    <div>
      <Modal
        title="Add new Certificate"
        opened={showConfirmation}
        onClose={setShowConfirmation}
        styles={{
          title: {
            fontWeight: "bold",
          },
        }}
      >
        <form>
          <TextInput label="Name" />
          <TextInput label="Duration" />
          <DateRangePicker
            label="Start Date - End Date"
            placeholder="Pick dates range"
            value={value}
            //@ts-ignore
            onChange={setValue}
          />
          <TextInput label="Location" />
          <TextInput label="Institute" />
        </form>
      </Modal>
      <Group position="right" onClick={() => setShowConfirmation()}>
        <Button>Add Certificate</Button>
      </Group>
      <Card withBorder mt={10}>
        <Flex justify={"space-between"}>
          <Box>
            <Text weight={"bold"}>Name: </Text>
            <Text weight={"bold"}>Duration:</Text>
            <Text weight={"bold"}>Date:</Text>
            <Text weight={"bold"}>Location:</Text>
            <Text weight={"bold"}>Institute:</Text>
          </Box>
          <Box style={{ width: 150, marginTop: "auto" }}>
            <div style={{ position: "relative" }}>
              <Image
                src={
                  "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b6/Image_created_with_a_mobile_phone.png/330px-Image_created_with_a_mobile_phone.png"
                }
                radius="md"
                style={{
                  opacity: "0.5",
                }}
              />
              <div
                style={{
                  position: "absolute",
                  left: 0,
                  bottom: 0,
                  right: 0,
                  margin: "auto",
                  top: 0,
                  width: "45px",
                  height: "30px",
                  display: "flex",
                }}
              >
                <ActionIcon onClick={() => console.log("View")} mr={10}>
                  <IconEye color="black" />
                </ActionIcon>
                <ActionIcon onClick={() => downloadImage("", "")}>
                  <IconDownload color="black" />
                </ActionIcon>
              </div>
            </div>
          </Box>
        </Flex>
        {auth?.auth && auth.auth.role <= UserRole.Admin && (
          <Box mt={10}>
            <Button>Approve</Button>
            <Button ml={10} variant="outline" color={"red"}>
              Reject
            </Button>
          </Box>
        )}
      </Card>
    </div>
  );
};

export default MyTrainingExternal;
