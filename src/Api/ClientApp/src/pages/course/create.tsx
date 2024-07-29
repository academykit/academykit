import TitleAndDescriptionSuggestion from '@components/Ui/AI/TitleAndDescriptionSuggestion';
import Breadcrumb from '@components/Ui/BreadCrumb';
import CustomTextFieldWithAutoFocus from '@components/Ui/CustomTextFieldWithAutoFocus';
import GroupCreatableSelect from '@components/Ui/GroupCreatableSelect';
import RichTextEditor from '@components/Ui/RichTextEditor/Index';
import ThumbnailEditor from '@components/Ui/ThumbnailEditor';
import useAuth from '@hooks/useAuth';
import useCustomForm from '@hooks/useCustomForm';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import {
  Accordion,
  ActionIcon,
  Box,
  Button,
  Checkbox,
  Flex,
  Group,
  Loader,
  Select,
  Text,
} from '@mantine/core';
import { DatePickerInput } from '@mantine/dates';
import { createFormContext, yupResolver } from '@mantine/form';
import { useScrollIntoView } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import { IconPlus, IconTrash } from '@tabler/icons-react';
import { TrainingEligibilityEnum } from '@utils/enums';
import queryStringGenerator from '@utils/queryStringGenerator';
import RoutePath from '@utils/routeConstants';
import { useDepartmentSetting } from '@utils/services/adminService';
import { useAIMaster, useTrainingSuggestion } from '@utils/services/aiService';
import { useAssessments } from '@utils/services/assessmentService';
import errorType from '@utils/services/axiosError';
import {
  IBaseTrainingEligibility,
  useCourse,
  useCreateCourse,
} from '@utils/services/courseService';
import { useAddGroup, useGroups } from '@utils/services/groupService';
import { useLevels } from '@utils/services/levelService';
import { useSkills } from '@utils/services/skillService';
import { ITag, useAddTag, useTags } from '@utils/services/tagService';
import moment from 'moment';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate, useSearchParams } from 'react-router-dom';
import * as Yup from 'yup';
import TagMultiSelectCreatable from './component/TagMultiSelectCreatable';

interface FormValues {
  thumbnail: string;
  title: string;
  level: any;
  groups: string;
  description: string;
  tags: string[];
  language: string;
  startDate: Date | null;
  endDate: Date | null;
  isUnlimitedEndDate: boolean;
  trainingEligibilities: IBaseTrainingEligibility[];
}
const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    title: Yup.string()
      .trim()
      .required(t('course_title_required') as string)
      .max(100, t('course_title_must_be_less_than_100') as string),
    level: Yup.string().required(t('level_required') as string),
    groups: Yup.string()
      .nullable()
      .required(t('group_required') as string),
    startDate: Yup.string()
      .required(t('start_date_required') as string)
      .typeError(t('start_date_required') as string),
    isUnlimitedEndDate: Yup.boolean(),
    endDate: Yup.string().when('isUnlimitedEndDate', {
      is: false,
      then: Yup.string()
        .required(t('end_date_required') as string)
        .typeError(t('end_date_required') as string),
      otherwise: Yup.string().nullable(),
    }),
    trainingEligibilities: Yup.array().of(
      Yup.object().shape({
        eligibilityId: Yup.string().required(t('field_required') as string),
      })
    ),
  });
};

export const [FormProvider, useFormContext, useForm] =
  createFormContext<FormValues>();

const CreateCoursePage = () => {
  const aiSuggestion = useTrainingSuggestion();
  const aiStatus = useAIMaster();
  const cForm = useCustomForm();
  const getDepartments = useDepartmentSetting(
    queryStringGenerator({ size: 1000 })
  );
  const skillData = useSkills(queryStringGenerator({ size: 1000 }));
  const getAssessments = useAssessments(queryStringGenerator({ size: 1000 }));
  const getTrainings = useCourse(queryStringGenerator({ size: 1000 }));
  const [searchParamGroup] = useState('');
  const { t } = useTranslation();
  const groupAdd = useAddGroup();
  const auth = useAuth();
  const [language] = useState([{ value: '1', label: 'English' }]);
  const { scrollIntoView: scrollToTop, targetRef: refBasic } =
    useScrollIntoView<HTMLDivElement>({
      offset: 60,
    });

  const groups = useGroups(
    queryStringGenerator({
      search: searchParamGroup,
      size: 10000,
    })
  );
  const [searchParams] = useSearchParams();
  const groupSlug = searchParams.get('group');
  useEffect(() => {
    if (groups.isSuccess && groups?.data && groupSlug) {
      form.setFieldValue(
        'groups',
        (
          groups.data &&
          groups.data.data.items.find((x) => x.slug === groupSlug)
        )?.id ?? ''
      );
    }
  }, [groups.isSuccess]);

  const form = useForm({
    initialValues: {
      thumbnail: '',
      title: '',
      level: '',
      groups: '',
      description: '',
      tags: [],
      language: '1',
      startDate: null,
      endDate: null,
      isUnlimitedEndDate: false,
      trainingEligibilities: [],
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const getDepartmentDropdown = () => {
    return getDepartments.data?.items.map((x) => ({
      value: x.id,
      label: x.name,
    }));
  };

  const getSkillDropdown = () => {
    return skillData.data?.items.map((skill) => ({
      value: skill.id,
      label: skill.skillName,
    }));
  };

  const getAssessmentDropdown = () => {
    return getAssessments.data?.items.map((assessment) => ({
      value: assessment.id,
      label: assessment.title,
    }));
  };

  const getTrainingDropdown = () => {
    return getTrainings.data?.items.map((training) => ({
      value: training.id,
      label: training.name,
    }));
  };

  const getEligibilityType = () => {
    return Object.entries(TrainingEligibilityEnum)
      .splice(0, Object.entries(TrainingEligibilityEnum).length / 2)
      .map(([key, value]) => ({
        value: key,
        label: t(value.toString()),
      }));
  };

  const [searchParam] = useState('');

  const label = useLevels();
  const { data: addTagData, isSuccess, mutateAsync: mutAsync } = useAddTag();

  const navigate = useNavigate();

  const tags = useTags(
    queryStringGenerator({
      search: searchParam,
      size: 10000,
    })
  );

  const [tagsLists, setTagsLists] = useState<ITag[]>([]);
  useEffect(() => {
    if (tags.isSuccess && tags.isFetched) {
      // setTagsList(tags.data.items.map((x) => ({ label: x.name, value: x.id })));
      setTagsLists(tags.data.items.map((x) => x));
    }
  }, [tags.isSuccess]);

  useEffect(() => {
    if (isSuccess) {
      // setTagsList([
      //   ...tagsList,
      //   { label: addTagData.data.name, value: addTagData.data.id },
      // ]);
      setTagsLists([...tagsLists, addTagData.data]);
      form.setFieldValue('tags', [...form.values.tags, addTagData?.data?.id]);
    }
  }, [isSuccess]);

  const { mutateAsync, isLoading } = useCreateCourse();
  const submitHandler = async (data: FormValues) => {
    try {
      const res = await mutateAsync({
        description: data.description,
        groupId: data.groups,
        tagIds: data.tags,
        levelId: data.level,
        language: parseInt(data.language),
        name: data.title.trim().split(/ +/).join(' '),
        thumbnailUrl: data.thumbnail,
        startDate: moment(data.startDate)
          .add(5, 'hour')
          .add(45, 'minute')
          .toISOString(),
        endDate: moment(data.endDate)
          .add(5, 'hour')
          .add(45, 'minute')
          .toISOString(),
        isUnlimitedEndDate: data.isUnlimitedEndDate,
        trainingEligibilities: data.trainingEligibilities.map((eligibility) => {
          return {
            eligibility: Number(eligibility.eligibility),
            eligibilityId: eligibility.eligibilityId,
          };
        }),
      });
      form.reset();
      showNotification({
        title: t('success'),
        message: t('create_training_success'),
      });
      navigate(RoutePath.manageCourse.lessons(res.data.slug).route);
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: 'red',
      });
    }
  };

  // set endDate as null if unlimited endDate is checked
  useEffect(() => {
    if (form.values.isUnlimitedEndDate) {
      form.setFieldValue('endDate', null);
    }
  }, [form.values.isUnlimitedEndDate]);

  const acceptAIAnswer = () => {
    form.setFieldValue('title', aiSuggestion.data?.title ?? '');
    form.setFieldValue('description', aiSuggestion.data?.description ?? '');
  };

  // scroll to error section
  const handleError = (errors: typeof form.errors) => {
    if (
      errors.title ||
      errors.level ||
      errors.groups ||
      errors.startDate ||
      errors.endDate
    ) {
      scrollToTop({
        alignment: 'center',
      });
    }
  };

  return (
    <div>
      <Breadcrumb />
      <FormProvider form={form}>
        <form onSubmit={form.onSubmit(submitHandler, handleError)}>
          <Box mt={20}>
            <ThumbnailEditor
              formContext={useFormContext}
              label={t('thumbnail') as string}
            />

            {aiStatus.data?.isActive &&
              aiStatus.data.key !== null &&
              aiStatus.data.key !== '' && (
                <TitleAndDescriptionSuggestion
                  title={aiSuggestion.data?.title}
                  description={aiSuggestion.data?.description}
                  isLoading={
                    aiSuggestion.isLoading ||
                    aiSuggestion.isFetching ||
                    aiSuggestion.isRefetching
                  }
                  refetch={() => aiSuggestion.refetch()}
                  acceptAnswer={() => acceptAIAnswer()}
                />
              )}

            <Group mt={10} grow ref={refBasic}>
              <CustomTextFieldWithAutoFocus
                placeholder={t('title_course') as string}
                label={t('title')}
                name="Title"
                withAsterisk
                {...form.getInputProps('title')}
                size="lg"
              />
            </Group>

            <Group grow mt={20}>
              {tags.isSuccess ? (
                <TagMultiSelectCreatable
                  data={tagsLists ?? []}
                  mutateAsync={mutAsync}
                  form={form}
                  size="lg"
                />
              ) : (
                <div>
                  <Loader />
                </div>
              )}
              {label.isSuccess ? (
                <Select
                  withAsterisk
                  size="lg"
                  placeholder={t('level_placeholder') as string}
                  label={t('level')}
                  {...form.getInputProps('level')}
                  data={
                    label.data.length > 0
                      ? label.data.map((x) => ({ value: x.id, label: x.name }))
                      : [
                          {
                            label: t('no_level') as string,
                            value: 'null',
                            disabled: true,
                          },
                        ]
                  }
                  styles={{ error: { position: 'absolute' } }}
                ></Select>
              ) : (
                <div>
                  <Loader />
                </div>
              )}
            </Group>

            <Group grow mt={20}>
              {!groups.isLoading ? (
                <GroupCreatableSelect
                  api={groupAdd}
                  form={form}
                  data={groups?.data?.data?.items}
                  size="lg"
                  auth={auth}
                />
              ) : (
                <Loader style={{ flexGrow: '0' }} />
              )}

              <Select
                label={t('Language')}
                size={'lg'}
                data={language}
                {...form.getInputProps('language')}
              />
            </Group>

            <Group grow mt={20}>
              <DatePickerInput
                withAsterisk
                minDate={new Date()}
                label={t('start_date')}
                placeholder={t('pick_date') as string}
                size="lg"
                {...form.getInputProps('startDate')}
              />

              <DatePickerInput
                minDate={form.values.startDate ?? new Date()}
                withAsterisk={!form.values.isUnlimitedEndDate}
                disabled={form.values.isUnlimitedEndDate}
                label={t('end_date')}
                placeholder={t('pick_date') as string}
                size="lg"
                {...form.getInputProps('endDate')}
              />
            </Group>
            <Group grow mt={20}>
              <Checkbox
                label="Unlimited end date"
                {...form.getInputProps('isUnlimitedEndDate', {
                  type: 'checkbox',
                })}
              />
            </Group>

            <Box mt={20}>
              <Text>{t('description')}</Text>
              <RichTextEditor
                placeholder={t('course_description') as string}
                formContext={useFormContext}
              />
            </Box>

            <Accordion defaultValue="Eligibility" mt={10}>
              <Accordion.Item value="Eligibility">
                <Accordion.Control>
                  {t('eligibility_criteria')}
                </Accordion.Control>
                <Accordion.Panel>
                  {form.values.trainingEligibilities.length < 1 && (
                    <Button
                      onClick={() => {
                        form.insertListItem(
                          'trainingEligibilities',
                          { eligibility: 0, eligibilityId: '' },
                          0
                        );
                      }}
                    >
                      {t('add_eligibility_criteria')}
                    </Button>
                  )}

                  {form.values.trainingEligibilities.map(
                    (_eligibility, index) => (
                      <Flex gap={10} key={index} align={'flex-end'} mb={10}>
                        <Select
                          allowDeselect={false}
                          label={t('eligibility_type')}
                          placeholder={t('pick_value') as string}
                          data={getEligibilityType() ?? []}
                          {...form.getInputProps(
                            `trainingEligibilities.${index}.eligibility`
                          )}
                        />
                        {form.values.trainingEligibilities[index].eligibility ==
                          TrainingEligibilityEnum.Department && (
                          <Select
                            withAsterisk
                            allowDeselect={false}
                            label={t('department')}
                            placeholder={t('pick_value') as string}
                            data={getDepartmentDropdown() ?? []}
                            {...form.getInputProps(
                              `trainingEligibilities.${index}.eligibilityId`
                            )}
                          />
                        )}

                        {form.values.trainingEligibilities[index].eligibility ==
                          TrainingEligibilityEnum.Training && (
                          <Select
                            withAsterisk
                            allowDeselect={false}
                            label={t('training')}
                            placeholder={t('pick_value') as string}
                            data={getTrainingDropdown() ?? []}
                            {...form.getInputProps(
                              `trainingEligibilities.${index}.eligibilityId`
                            )}
                          />
                        )}

                        {form.values.trainingEligibilities[index].eligibility ==
                          TrainingEligibilityEnum.Skills && (
                          <Select
                            withAsterisk
                            allowDeselect={false}
                            label={t('skills')}
                            placeholder={t('pick_value') as string}
                            data={getSkillDropdown() ?? []}
                            {...form.getInputProps(
                              `trainingEligibilities.${index}.eligibilityId`
                            )}
                          />
                        )}

                        {form.values.trainingEligibilities[index].eligibility ==
                          TrainingEligibilityEnum.Assessment && (
                          <Select
                            withAsterisk
                            allowDeselect={false}
                            label={t('assessment')}
                            placeholder={t('pick_value') as string}
                            data={getAssessmentDropdown() ?? []}
                            {...form.getInputProps(
                              `trainingEligibilities.${index}.eligibilityId`
                            )}
                          />
                        )}

                        <ActionIcon
                          variant="subtle"
                          onClick={() => {
                            form.insertListItem(
                              'trainingEligibilities',
                              { eligibility: 0, eligibilityId: '' },
                              index + 1
                            );
                          }}
                        >
                          <IconPlus />
                        </ActionIcon>

                        <ActionIcon
                          variant="subtle"
                          c={'red'}
                          onClick={() => {
                            form.removeListItem('trainingEligibilities', index);
                          }}
                        >
                          <IconTrash />
                        </ActionIcon>
                      </Flex>
                    )
                  )}
                </Accordion.Panel>
              </Accordion.Item>
            </Accordion>

            <Box mt={20}>
              <Button
                disabled={!cForm?.isReady}
                size="lg"
                type="submit"
                loading={isLoading}
              >
                {t('submit')}
              </Button>
            </Box>
          </Box>
        </form>
      </FormProvider>
    </div>
  );
};

export default CreateCoursePage;
