import {
  Avatar,
  Box,
  createStyles,
  Group,
  Paper,
  Tabs,
  Text,
} from "@mantine/core";
import { IconEdit, IconFileDescription, IconSchool } from "@tabler/icons";
import { useProfileAuth } from "@utils/services/authService";
import { useTranslation } from "react-i18next";
import {
  Link,
  Outlet,
  useLocation,
  useNavigate,
  useParams,
} from "react-router-dom";
import TextViewer from "@components/Ui/RichTextViewer";

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
  const location = useLocation();

  const { data, isSuccess } = useProfileAuth(id as string);
  const currentLocation = location.pathname.split('/').slice(-1)[0] // accessing the endpoint of the url

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
                <Link to={"/settings?edit=1"}>
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
              <TextViewer
                styles={{
                  root: {
                    border: "none",
                  },
                }}
                content={data?.bio}
              />
            </>
          )}
        </Paper>
      </div>
      <Box mt={20}>
        <Tabs
          defaultChecked={true}
          defaultValue={location.pathname?.split("/").at(-1) ?? "certificate"}
          value={currentLocation}
          onTabChange={(value) =>
            navigate(`${value}`, { preventScrollReset: true })
          }
        >
          <Tabs.List>
            <Tabs.Tab
              value="certificate"
              icon={<IconFileDescription size={14} />}
            >
              {t("only_certificate")}
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
