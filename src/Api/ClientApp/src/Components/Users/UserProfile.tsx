import {
  ActionIcon,
  Anchor,
  Avatar,
  Badge,
  CopyButton,
  createStyles,
  Divider,
  Flex,
  Group,
  Image,
  Modal,
  Paper,
  ScrollArea,
  Table,
  Text,
  Title,
  Tooltip,
} from "@mantine/core";
import { IconCheck, IconCopy, IconDownload, IconEdit } from "@tabler/icons";
import { useProfileAuth } from "@utils/services/authService";

import { Link, useParams } from "react-router-dom";
import RichTextEditor from "@mantine/rte";
import { ICertificateList } from "@utils/services/manageCourseService";
import useAuth from "@hooks/useAuth";
import { UserRole } from "@utils/enums";
import { useState } from "react";

const useStyles = createStyles((theme) => ({
  avatarImage: {
    height: "200px",
  },
  avatar: {
    display: "flex",
    alignItems: "center",
    [theme.fn.smallerThan("sm")]: {
      flexDirection: "column",
      // flexWrap: "wrap",
    },
  },
}));

const RowsCompleted = ({ item }: { item: ICertificateList }) => {
  const [opened, setOpened] = useState(false);
  const handleDownload = () => {
    window.open(item?.certificateUrl);
  };
  return (
    <tr key={item?.user?.id}>
      <td>{item?.courseName}</td>

      <td>{item?.percentage}%</td>
      <td>
        {item?.hasCertificateIssued ? <Badge>Yes</Badge> : <Badge>No</Badge>}
      </td>
      <td style={{ maxWidth: "0px" }}>
        <Modal
          opened={opened}
          size="xl"
          title={item?.courseName}
          onClose={() => setOpened(false)}
        >
          <Image src={item?.certificateUrl}></Image>
        </Modal>
        <Flex align={"center"}>
          <Anchor onClick={() => setOpened((v) => !v)}>
            <Image
              width={150}
              height={100}
              fit="contain"
              // sx={{":hover"}}
              src={item?.certificateUrl}
            />
          </Anchor>
          <CopyButton value={item?.certificateUrl} timeout={2000}>
            {({ copied, copy }) => (
              <Tooltip
                label={copied ? "Copied" : "Copy"}
                withArrow
                position="right"
              >
                <ActionIcon color={copied ? "teal" : "gray"} onClick={copy}>
                  {copied ? <IconCheck size={16} /> : <IconCopy size={16} />}
                </ActionIcon>
              </Tooltip>
            )}
          </CopyButton>
          <ActionIcon onClick={() => handleDownload()}>
            <IconDownload />
          </ActionIcon>
        </Flex>
      </td>
    </tr>
  );
};

const UserProfile = () => {
  const { classes } = useStyles();
  const query = useParams();
  const local_id = localStorage.getItem("id");
  const auth = useAuth();

  const { data, isLoading, isSuccess } = useProfileAuth(query.id as string);
  return (
    <>
      <div>
        <div className={classes.avatar}>
          <Avatar
            src={data?.imageUrl}
            size={200}
            sx={{ borderRadius: "50%" }}
            alt={data?.fullName}
          />

          <div style={{ marginLeft: "15px" }}>
            <Group>
              <Text size={"xl"}>{data?.fullName}</Text>
              {isSuccess && query.id === local_id ? (
                <Link to={"/settings"}>
                  <IconEdit style={{ marginLeft: "5px" }} />
                </Link>
              ) : (
                ""
              )}
            </Group>
            {`${data?.profession ?? ""}`}
          </div>
        </div>
        <Paper shadow={"lg"} withBorder sx={{ marginTop: "5px" }}>
          <Text
            align="center"
            size={"xl"}
            sx={{ margin: "5px 0", padding: "3px" }}
          >
            About
          </Text>
          <Divider />
          <Text size={"md"} sx={{ padding: "5px 50px" }}>
            Address: {data?.address}
          </Text>
          <Text size={"md"} sx={{ padding: "5px 50px" }}>
            Mobile number: {data?.mobileNumber}
          </Text>
          <Text size={"md"} sx={{ padding: "5px 50px" }}>
            Email: {data?.email}
          </Text>
          {data?.bio && (
            <>
              <Text size={"md"} sx={{ padding: "5px 50px" }}>
                Bio:
              </Text>
              <RichTextEditor
                mt={1}
                mb={15}
                m={50}
                color="dimmed"
                readOnly
                value={data?.bio}
              />
            </>
          )}
        </Paper>
      </div>
      {auth?.auth &&
        auth?.auth.role === UserRole.Trainee &&
        data?.certificates &&
        data?.certificates?.length > 0 && (
          <>
            <Title mt={"xl"}>Certificate</Title>
            <ScrollArea>
              <Paper mt={10}>
                <Table
                  sx={{ minWidth: 800 }}
                  verticalSpacing="sm"
                  striped
                  highlightOnHover
                >
                  <thead>
                    <tr>
                      <th>Trainings Name</th>
                      <th>Completion</th>
                      <th>isIssued</th>
                      <th>Certificate URL</th>
                    </tr>
                  </thead>
                  <tbody>
                    {data?.certificates &&
                      data?.certificates.map((x: any) => (
                        <RowsCompleted key={x.userId} item={x} />
                      ))}
                  </tbody>
                </Table>
              </Paper>
            </ScrollArea>
          </>
        )}
    </>
  );
};

export default UserProfile;
