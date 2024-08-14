import TrainingTable from "@components/Course/TrainingTable";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useAuth from "@hooks/useAuth";
import {
  Box,
  Center,
  Container,
  Flex,
  Group,
  Loader,
  rem,
  SegmentedControl,
} from "@mantine/core";
import { IconColumns, IconLayoutGrid } from "@tabler/icons-react";
import { CourseStatus, UserRole } from "@utils/enums";
import { useCourse } from "@utils/services/courseService";
import { useEffect, useState } from "react"; // Import useState
import { useTranslation } from "react-i18next";
import CourseList from "./component/List";

const ReviewedCourse = ({
  setInitialSearch,
  searchParams,
  searchComponent,
  pagination,
}: IWithSearchPagination) => {
  useEffect(() => {
    setInitialSearch([
      {
        key: "Status",
        value: CourseStatus.Review.toString(),
      },
    ]);
  }, []);
  const auth = useAuth();
  const { data, isLoading } = useCourse(searchParams);
  const role = auth?.auth?.role ?? UserRole.Trainee;
  const { t } = useTranslation();
  const [selectedView, setSelectedView] = useState("list");

  const [showCourseList, setShowCourseList] = useState(false);

  useEffect(() => {
    const timer = setTimeout(() => {
      setShowCourseList(true);
    }, 1000); // 1 second

    return () => clearTimeout(timer);
  }, []);

  return (
    <Container fluid>
      <Container fluid>
        <Flex
          pb={20}
          style={{
            justifyContent: "end",
            alignItems: "center",
          }}
        >
          {searchComponent(t("search_trainings") as string)}
        </Flex>
      </Container>

      <Group justify="flex-end" my={30}>
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

      {data &&
        data?.items &&
        (data.totalCount >= 1 ? (
          // Show CourseList based on the state value
          showCourseList ? (
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
            <Loader />
          )
        ) : (
          <Box>{t("no_trainings_found")}</Box>
        ))}
      {isLoading && <Loader />}
      {selectedView === "table" &&
        showCourseList &&
        data &&
        pagination(data.totalPage, data.items.length)}
    </Container>
  );
};

export default withSearchPagination(ReviewedCourse);
