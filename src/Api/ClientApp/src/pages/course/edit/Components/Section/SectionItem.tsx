import DeleteModal from "@components/Ui/DeleteModal";
import EditNameForm from "@components/Ui/EditNameForm";
import { useSection } from "@context/SectionProvider";
import { Button, Container, Group, Modal, Paper } from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconChevronRight, IconPencilMinus, IconTrashX } from "@tabler/icons";
import errorType from "@utils/services/axiosError";
import {
  ISection,
  useDeleteSection,
  useUpdateSectionName,
} from "@utils/services/courseService";
import { useState } from "react";
import Lessons from "./Lessons";

const SectionItem = ({ item, slug }: { item: ISection; slug: string }) => {
  const [value, toggle] = useToggle();
  const [isEditing, setIsEditing] = useState<boolean>(false);
  const updateSection = useUpdateSectionName(slug);
  const deleteSection = useDeleteSection(slug);
  const section = useSection();

  const onDelete = async () => {
    try {
      await deleteSection.mutateAsync({
        id: slug,
        sectionId: item.slug,
      });
      showNotification({
        message: "Section Deleted successfully!",
        title: "Success",
      });
      toggle();
    } catch (error: any) {
      const err = errorType(error);
      showNotification({
        message: err,
        title: "Error",
      });
    }
  };
  const active = () => section?.activeSection === item.slug;

  return (
    <div style={{ marginTop: "20px" }} key={item.id}>
      <DeleteModal
        title={`Are you sure you want to delete?`}
        open={value}
        onClose={toggle}
        onConfirm={onDelete}
      />

      {/* <Droppable droppableId={item.id} type="courses"> */}
      {/* {(provided) => ( */}
      <Paper
        // ref={provided.innerRef}
        // {...provided.droppableProps}
        withBorder
        p={10}
      >
        <Container
          fluid
          style={{
            display: "flex",
            justifyContent: "space-between",
          }}
        >
          {!isEditing ? (
            <div>
              {item.name}
              <IconPencilMinus
                size={16}
                style={{ marginLeft: "10px", cursor: "pointer" }}
                onClick={() => {
                  setIsEditing(true);
                }}
              />
            </div>
          ) : (
            <EditNameForm
              updateFunction={updateSection}
              item={item}
              slug={slug}
              setIsEditing={setIsEditing}
            />
          )}
          <Group position="right" grow>
            <IconTrashX
              size={18}
              style={{ color: "red", cursor: "pointer" }}
              onClick={() => {
                toggle();
              }}
            />
            <IconChevronRight
              size={16}
              onClick={() => {
                section?.setActiveSection(item.slug);
              }}
              style={{
                transform: active() ? "rotate(90deg)" : "",
                transition: "0.35s",
                cursor: "pointer",
              }}
            />
          </Group>
        </Container>
        {active() && item.lessons && (
          <Lessons lessons={item.lessons} sectionId={item.slug} />
        )}
        {/* {provided.placeholder} */}
      </Paper>
      {/* )} */}
      {/* </Droppable> */}
    </div>
  );
};

export default SectionItem;
