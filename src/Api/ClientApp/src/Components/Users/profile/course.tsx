import ProgressBar from "@components/Ui/ProgressBar";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import { Anchor, Table, Title, useMantineTheme, Box } from "@mantine/core";
import { useMyCourse } from "@utils/services/courseService";
import moment from "moment";
import { Link, useParams } from "react-router-dom";

const UserCourse = (props: IWithSearchPagination) => {
  const { id } = useParams();
  const { data } = useMyCourse(id as string);
  const theme = useMantineTheme();

  return (
    <>
      <>
        <Title mt={10} size={30}>
          Attended Trainings{" "}
        </Title>
        {data && data.totalCount > 0 && (
          <Table>
            <thead>
              <tr>
                <th>Title</th>
                <th>Enrolled Date</th>
                <th>Progress</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map((x) => (
                <tr>
                  <td>
                    <Anchor component={Link} to={`/trainings/${x.slug}`}>
                      {x.name}
                    </Anchor>
                  </td>
                  <td>{moment(x.createdOn).format(theme.dateFormat)}</td>
                  <td>
                    <ProgressBar total={100} positive={x.percentage} />
                  </td>
                </tr>
              ))}
            </tbody>
          </Table>
        )}
        {data && data.totalCount === 0 && <Box mt={5}>No Trainings Found</Box>}
      </>
      {/* <>
        <Title mt={20} size={30}>
          My Trainings
        </Title>
      </> */}
    </>
  );
};

export default withSearchPagination(UserCourse);
