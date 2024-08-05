import {
  ActionIcon,
  Anchor,
  Badge,
  Box,
  Loader,
  Pagination,
  Paper,
  ScrollArea,
  Table,
  Title,
  Tooltip,
} from "@mantine/core";
import { IconEdit } from "@tabler/icons-react";
import { DATE_FORMAT } from "@utils/constants";
import { CourseLanguage, UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { useReAuth } from "@utils/services/authService";
import { useCourse } from "@utils/services/courseService";
import moment from "moment";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Link, useParams } from "react-router-dom";

const MyTrainings = () => {
  const auth = useReAuth();
  const [page, setPage] = useState(1);
  const { id } = useParams();
  const { t } = useTranslation();
  const authorCourse = useCourse(
    `UserId=${id}&Enrollmentstatus=1&size=12&page=${page}`
  );

  return (
    <>
      {auth.data && Number(auth.data?.role) !== UserRole.Trainee && (
        <div>
          <Title mt={20} size={30} mb={10}>
            {t("my_trainings")}
          </Title>

          <ScrollArea>
            <Paper>
              <Table striped withTableBorder withColumnBorders highlightOnHover>
                <Table.Thead>
                  <Table.Tr>
                    <Table.Th>{t("title")}</Table.Th>
                    <Table.Th>{t("created_date")}</Table.Th>
                    <Table.Th>
                      {t("Language")} / {t("level")}
                    </Table.Th>
                    <Table.Th>{t("action")}</Table.Th>
                  </Table.Tr>
                </Table.Thead>
                <Table.Tbody>
                  {authorCourse.data &&
                    authorCourse.data.totalCount > 0 &&
                    authorCourse.data.items.map((x) => (
                      <Table.Tr key={x.id}>
                        <Table.Td>
                          <Anchor
                            component={Link}
                            to={RoutePath.courses.description(x.slug).route}
                          >
                            {x.name}
                          </Anchor>
                        </Table.Td>
                        <Table.Td>
                          {moment(x.createdOn).format(DATE_FORMAT)}
                        </Table.Td>
                        <Table.Td>
                          <Badge color="pink" variant="light">
                            {CourseLanguage[x.language]}
                          </Badge>{" "}
                          /{" "}
                          <Badge color="blue" variant="light">
                            {x?.levelName}
                          </Badge>
                        </Table.Td>
                        <Table.Td>
                          <Tooltip label={t("edit_this_course")}>
                            <ActionIcon
                              component={Link}
                              to={RoutePath.manageCourse.edit(x.slug).route}
                              color="gray"
                              variant="subtle"
                            >
                              <IconEdit />
                            </ActionIcon>
                          </Tooltip>
                        </Table.Td>
                      </Table.Tr>
                    ))}
                  {authorCourse.isLoading && <Loader />}
                </Table.Tbody>
              </Table>
            </Paper>
          </ScrollArea>

          {
            authorCourse.data && authorCourse.data.totalPage > 1 && (
              <Pagination
                my={20}
                total={authorCourse.data.totalPage}
                value={page}
                onChange={setPage}
              />
            )
            // pagination(authorCourse.data.totalPage)
          }

          {authorCourse.data && authorCourse.data.totalCount === 0 && (
            <Box mt={5}>{t("no_trainings_found")}</Box>
          )}
        </div>
      )}
    </>
  );
};

export default MyTrainings;
