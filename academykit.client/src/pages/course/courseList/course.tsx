import TrainingTable from "@components/Course/TrainingTable";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useAuth from "@hooks/useAuth";
import {
  Box,
  Button,
  Center,
  Flex,
  Group,
  Loader,
  rem,
  ScrollArea,
  SegmentedControl,
  Title,
} from "@mantine/core";
import { IconColumns, IconLayoutGrid } from "@tabler/icons-react";
import { CourseStatus, CourseUserStatus, UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { useCourse } from "@utils/services/courseService";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import CourseList from "./component/List";

const CoursePage = ({
  filterComponent,
  searchParams,
  pagination,
  searchComponent,
  multiFilterComponent,
}: IWithSearchPagination) => {
  const { data, isLoading } = useCourse(searchParams);
  const [selectedView, setSelectedView] = useState("list");
  const auth = useAuth();
  const role = auth?.auth?.role ?? UserRole.Trainee;
  const { t } = useTranslation();

  const filterValue = [
    {
      value: CourseUserStatus.Author.toString(),
      label: t("author"),
    },
    {
      value: CourseUserStatus.Enrolled.toString(),
      label: t("enrolled"),
    },
    {
      value: CourseUserStatus.NotEnrolled.toString(),
      label: t("not_enrolled"),
    },
    {
      value: CourseUserStatus.Teacher.toString(),
      label: t("trainer"),
    },
  ];

  const filterStatusValue = [
    {
      value: CourseStatus.Draft.toString(),
      label: t("Draft"),
    },
    {
      value: CourseStatus.Review.toString(),
      label: t("review"),
    },
    {
      value: CourseStatus.Published.toString(),
      label: t("Published"),
    },
    {
      value: CourseStatus.Archived.toString(),
      label: t("Archived"),
    },
    {
      value: CourseStatus.Rejected.toString(),
      label: t("rejected"),
    },
    {
      value: CourseStatus.Completed.toString(),
      label: t("completed"),
    },
  ];

  return (
    <>
      <Box style={{ justifyContent: "space-between", alignItems: "center" }}>
        <Group justify="space-between">
          <Title style={{ flexGrow: 2 }}>{t("trainings")}</Title>
          {role != UserRole.Trainee && (
            <Link to={RoutePath.courses.create}>
              <Button ml={5}>{t("new_training")}</Button>
            </Link>
          )}
        </Group>
      </Box>
      <Group my={10}>
        <Box flex={1}>{searchComponent(t("search_trainings") as string)}</Box>
        {multiFilterComponent(
          filterValue,
          t("enrollment_status"),
          "Enrollmentstatus"
        )}
        {filterComponent(filterStatusValue, t("status"), "Status")}

        <SegmentedControl
          value={selectedView}
          onChange={setSelectedView}
          data={[
            {
              value: "list",
              label: (
                <Center style={{ gap: 10 }}>
                  <IconLayoutGrid style={{ width: rem(20), height: rem(20) }} />
                </Center>
              ),
            },
            {
              value: "table",
              label: (
                <Center style={{ gap: 10 }}>
                  <IconColumns style={{ width: rem(20), height: rem(20) }} />
                </Center>
              ),
            },
          ]}
        />
      </Group>
      <ScrollArea>
        {data &&
          data?.items &&
          (data.totalCount >= 1 ? (
            selectedView === "table" ? (
              <TrainingTable courses={data?.items} search={searchParams} />
            ) : (
              <CourseList
                role={role}
                courses={data.items}
                search={searchParams}
              />
            )
          ) : (
            <Box>{t("no_trainings_found")}</Box>
          ))}
      </ScrollArea>
      {isLoading && <Flex justify={"center"}>{<Loader mx={"auto"} />}</Flex>}
      {selectedView === "table" &&
        data &&
        pagination(data.totalPage, data.items.length)}
    </>
  );
};

export default withSearchPagination(CoursePage);
