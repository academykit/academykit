import useAuth from "@hooks/useAuth";
import {
  Box,
  Button,
  Card,
  Flex,
  Group,
  Image,
  Popover,
  Text,
  Paper,
  NavLink,
  Avatar,
  AspectRatio,
  useMantineTheme,
} from "@mantine/core";
import { IconChevronRight, IconDotsVertical } from "@tabler/icons";
import { UserRole } from "@utils/enums";
import getCourseOgImageUrl from "@utils/getCourseOGImage";
import { getInitials } from "@utils/getInitialName";
import RoutePath from "@utils/routeConstants";
import { DashboardCourses } from "@utils/services/dashboardService";
import { IUser } from "@utils/services/types";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

const StudentAvatar = ({ data }: { data: IUser }) => {
  return (
    <Avatar src={data.imageUrl} radius="xl">
      {" "}
      {!data.imageUrl && getInitials(data.fullName ?? "")}
    </Avatar>
  );
};

const TrainingCards = ({ data }: { data: DashboardCourses }) => {
  const auth = useAuth();
  const role = auth?.auth?.role;
  const { t } = useTranslation();

  return (
    <Card
      sx={{
        ["@media (max-width: 400px)"]: {
          width: "100%",
        },
        position: "relative",
        overflow: "visible",
        width: "350px",
      }}
      withBorder
    >
      <Link
        to={RoutePath.courses.description(data.slug).route}
        style={{ position: "absolute", height: "100%", width: "100%" }}
      ></Link>
      <Flex sx={{ justifyContent: "start", alignItems: "start" }} gap={"sm"}>
        <Box sx={{ height: 65, width: 160 }}>
          <AspectRatio ratio={16 / 9}>
            <Image
              src={getCourseOgImageUrl(data.user, data.name, data.thumbnailUrl)}
              radius="sm"
              height={"100%"}
              width={"100%"}
              fit="contain"
            />
          </AspectRatio>
        </Box>
        <Box w={"100%"}>
          <Flex justify={"space-between"} sx={{ width: "100%" }}>
            <Text lineClamp={1} h={"95%"} weight="bold">
              {data.name}
            </Text>
            <div>
              {(role === UserRole.Admin || role === UserRole.SuperAdmin) && (
                <Popover
                  position={"left-start"}
                  arrowSize={12}
                  zIndex="100"
                  styles={{
                    dropdown: { padding: 5 },
                  }}
                >
                  <Popover.Target>
                    <Button sx={{ zIndex: 50 }} variant="subtle" px={4}>
                      <IconDotsVertical />
                    </Button>
                  </Popover.Target>
                  <Popover.Dropdown>
                    <Paper>
                      <Group
                        p={0}
                        sx={{
                          flexDirection: "column",
                          alignItems: "start",
                        }}
                      >
                        <NavLink
                          variant="subtle"
                          label={t("manage")}
                          component={Link}
                          to={RoutePath.manageCourse.manage(data.slug).route}
                          rightSection={
                            <IconChevronRight size={12} stroke={1.5} />
                          }
                        ></NavLink>
                      </Group>
                    </Paper>
                  </Popover.Dropdown>
                </Popover>
              )}
            </div>
          </Flex>
          {role === UserRole.Trainee ? (
            <Text size="sm">
              {data.percentage}% {t("progress")}
            </Text>
          ) : (
            <Avatar.Group spacing={"lg"}>
              {data.students.length > 0 ? (
                data.students.map((x, idx) => {
                  if (idx === 2) return;
                  return <StudentAvatar data={x} key={x.id} />;
                })
              ) : (
                <Text size="xs">{t("no_user_enrolled")}</Text>
              )}
              {data.students.length > 3 && (
                <Avatar radius="xl">+{data.students.length - 3}</Avatar>
              )}
            </Avatar.Group>
          )}
        </Box>
      </Flex>
    </Card>
  );
};

export default TrainingCards;
