import {
  Avatar,
  Box,
  createStyles,
  Divider,
  Group,
  Paper,
  Tabs,
  Text,
} from "@mantine/core";
import {
  IconEdit,
  IconFileDescription,
  IconSchool,
} from "@tabler/icons";
import { useProfileAuth } from "@utils/services/authService";
import { useTranslation } from "react-i18next";
import { Link, Outlet, useNavigate, useParams } from "react-router-dom";
import RichTextEditor from "@mantine/rte";


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
const UserProfile = () => {
  const { id, tabValue } = useParams();
  const { classes } = useStyles();
  const local_id = localStorage.getItem("id");
  const navigate = useNavigate();
  const { t } = useTranslation();

  const { data, isSuccess } = useProfileAuth(id as string);
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
              {isSuccess && id === local_id ? (
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
            {data?.fullName}
          </Text>
          <Divider />
          <Text size={"md"} sx={{ padding: "5px 50px" }}>
            {t("address")} : {data?.address}
          </Text>
          <Text size={"md"} sx={{ padding: "5px 50px" }}>
            {t("mobilenumber")} : {data?.mobileNumber}
          </Text>
          <Text size={"md"} sx={{ padding: "5px 50px" }} mb={10}>
            {t("email")} : {data?.email}
          </Text>
          {data && data.bio && data?.bio.replace(/<[^>]+>/g, "").length > 0 && (
            <>
              <RichTextEditor
                styles={{
                  root: {
                    border: "none",
                  },
                }}
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
      <Box mt={20}>
        <Tabs
          defaultChecked={true}
          defaultValue={"certificate"}
          value={tabValue}
          onTabChange={(value) =>
            navigate(`${value}`, { preventScrollReset: true })
          }
        >
          <Tabs.List>
            <Tabs.Tab
              value="certificate"
              icon={<IconFileDescription size={14} />}
            >
              {t("certificate")}
            </Tabs.Tab>
            <Tabs.Tab value="training" icon={<IconSchool size={14} />}>
              {t("training")}
            </Tabs.Tab>
          </Tabs.List>

          <Box pt="xs">
            <Outlet />
          </Box>
        </Tabs>
      </Box>
    </>
  );
};

export default UserProfile;
